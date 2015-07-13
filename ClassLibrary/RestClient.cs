using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using PCLCrypto;

namespace ClassLibrary
{
    public class RestClient
    {
        /// <summary>
        /// 
        /// </summary> 
        /// <returns>Results of downloading</returns>
        public async Task<byte[]> GetArchiveFileChecksumCheckAsync()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://excelconvertertest.azurewebsites.net/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var result = await client.GetAsync("api/GetZipDatabaseArchiveFile");
                if (!result.IsSuccessStatusCode) return null;
                var archive = await result.Content.ReadAsAsync<byte[]>();
                var hash = WinRTCrypto.HashAlgorithmProvider.OpenAlgorithm(HashAlgorithm.Sha256);
                var stringBuilder = new StringBuilder();
                foreach (var x in hash.HashData(archive))
                {
                    stringBuilder.Append(string.Format("{0:x2}", x));
                }
                var checksum = await GetArchiveChecksumAsync();
                Debug.WriteLine("hash " + stringBuilder + "\nchecksum " + checksum);
                return stringBuilder.ToString().Equals(checksum) ? archive : null;
            }
        }
        private static async Task<string> GetArchiveChecksumAsync()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                
                var result = await client.GetAsync("api/GetZipDatabaseArchiveChecksum");
                if (result.IsSuccessStatusCode)
                    return await result.Content.ReadAsAsync<string>();
                return null;
            }
        }
    }
}
