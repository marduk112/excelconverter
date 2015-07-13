using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using ExcelConverter.Logic;
using ExcelConverter.Repositories.Interfaces;

namespace ExcelConverter.Repositories.Implementations
{
    public class SQLiteRepository : DatabaseRepository, IExcelRepository
    {
        public Task<IEnumerable<string>> SaveExcelFileInDatabase(HttpRequestMessage request)
        {
            return SaveExcelFileInDatabase(request, Database.SQLite);
        }
    }
}