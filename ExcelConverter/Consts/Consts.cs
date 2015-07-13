using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ExcelConverter.Consts
{
    public static class Consts
    {
        /// <summary>
        /// Checks if path is virtual or not and always return physical path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetPath(string path)
        {
            path = path.Replace("|DataDirectory|",
                AppDomain.CurrentDomain.GetData("DataDirectory").ToString());
            return Path.IsPathRooted(path) ? path : HttpContext.Current.Server.MapPath(path);
        }
    }
}