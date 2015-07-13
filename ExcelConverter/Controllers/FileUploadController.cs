using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;
using ExcelConverter.Repositories.Interfaces;
using PCLCrypto;

namespace ExcelConverter.Controllers
{
        public class FileUploadController : ApiController
        {
            public FileUploadController(IExcelRepository excelRepository)
            {
                _excelRepository = excelRepository;
            }
            /// <summary>
            /// Upload and convert excel file
            /// </summary>
            /// <returns></returns>
            [HttpPost, Route("api/UploadConvertExcelFile")]
            public Task<IEnumerable<string>> UploadConvertExcelFileTask()
            {
                return _excelRepository.SaveExcelFileInDatabase(Request);
            }
            /// <summary>
            /// Return zip database archive checksum made by SHA256 algorithm by PCLCrypto library
            /// </summary>
            /// <returns></returns>
            [Route("api/GetZipDatabaseArchiveChecksum")]
            public string GetZipDatabaseArchiveChecksum()
            {
                var hash = WinRTCrypto.HashAlgorithmProvider.OpenAlgorithm(HashAlgorithm.Sha256);
                var stringBuilder = new StringBuilder();
                var archive = File.ReadAllBytes(PathToDatabaseArchiveFile);
                foreach (var x in hash.HashData(archive))
                {
                    stringBuilder.Append(string.Format("{0:x2}", x));
                }
                return stringBuilder.ToString();
            }
            /// <summary>
            /// Return zip database archive file
            /// </summary>
            /// <returns></returns>
            [Route("api/GetZipDatabaseArchiveFile")]
            public byte[] GetZipArchiveDatabaseFile()
            {
                return File.ReadAllBytes(PathToDatabaseArchiveFile);
            }

            private static readonly string PathToDatabaseArchiveFile = Consts.Consts.GetPath
                (WebConfigurationManager.AppSettings.Get("PathToDatabaseArchiveFile"));
            private readonly IExcelRepository _excelRepository;
        }
}
