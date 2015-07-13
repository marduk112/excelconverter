using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ExcelConverter.Repositories.Interfaces
{
    public interface IExcelRepository
    {
        Task<IEnumerable<string>> SaveExcelFileInDatabase(HttpRequestMessage request);

    }
}
