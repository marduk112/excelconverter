using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Http;
using ExcelConverter.Logic;

namespace ExcelConverter.Repositories
{
    public enum Database
    {
        SQLite,
    }
    //We must inherited from this class for differents database repositories
    public class DatabaseRepository
    {
        //based on http://www.codeguru.com/csharp/.net/uploading-files-asynchronously-using-asp.net-web-api.htm
        protected Task<IEnumerable<string>> SaveExcelFileInDatabase(HttpRequestMessage request, Database database)
        {
            if (!request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(request.CreateResponse(HttpStatusCode.NotAcceptable, "Invalid Request!"));

            var fullPath = Consts.Consts.GetPath(WebConfigurationManager.AppSettings.Get("PathToUploadFiles"));
            var streamProvider = new CustomMultipartFormDataStreamProvider(fullPath);
            var task = request.Content.ReadAsMultipartAsync(streamProvider).ContinueWith(t =>
            {
                if (t.IsFaulted || t.IsCanceled)
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);

                var fileInfo = streamProvider.FileData.Select(i =>
                {
                    var info = new FileInfo(i.LocalFileName);
                    var stopWatch = new Stopwatch();
                    stopWatch.Start();
                    try
                    {
                        if (!Path.GetExtension(info.FullName).Equals(".xlsx") &&
                            !Path.GetExtension(info.FullName).Equals(".xls"))
                            return "File " + info.Name + " isn't excel file";
                        var excelConverter = new Logic.ExcelConverter();
                        switch (database)
                        {
                            case Database.SQLite:
                                excelConverter.ConvertToSqlite(Consts.Consts.GetPath(info.FullName));
                                break;
                        }
                        
                    }
                    catch (Exception e)
                    {
                        stopWatch.Stop();
                        return e.Message + " elapsed Time in seconds " + stopWatch.ElapsedMilliseconds / 1000;
                    }
                    stopWatch.Stop();
                    return "File successfully converted " + info.Name + " (" + info.Length + ")"
                        + " elapsed Time in seconds " + stopWatch.ElapsedMilliseconds / 1000;
                });
                return fileInfo;
            });
            return task;
        }
    }
}