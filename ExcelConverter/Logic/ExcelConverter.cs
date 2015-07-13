using System;
using System.Data;
using System.Data.OleDb;
using System.Data.SQLite;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace ExcelConverter.Logic
{
    public class ExcelConverter
    {
        public async void ConvertToSqlite(string pathToExcelFile)
        {
            SetPathToParentDirectoryOfDatabaseFile();
            if (File.Exists(PathToDatabaseArchiveFile) && !File.Exists(_pathToDatabaseFile))
                ZipFile.ExtractToDirectory(PathToDatabaseArchiveFile, _pathToDatabase);
            using (
                var dbSqLiteConnection =
                    new SQLiteConnection((WebConfigurationManager.ConnectionStrings["SQLite"].ConnectionString)))
            {                
                //load data from xlsx(excel) file
                var ds = await SetDataSet(pathToExcelFile);
                await dbSqLiteConnection.OpenAsync();
                //Set data from rows
                for (var i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    var rowsStringBuilder = new StringBuilder();
                    //load data from row to string
                    for (var j = 0; j < ds.Tables[0].Rows[i].ItemArray.Length; j++)
                    {
                        var row = string.IsNullOrEmpty(ds.Tables[0].Rows[i][j].ToString())
                            ? "NULL"
                            : ds.Tables[0].Rows[i][j].ToString();
                        if (j < ds.Tables[0].Rows[i].ItemArray.Length - 1)
                            rowsStringBuilder.Append(row + ",");
                        else
                            rowsStringBuilder.Append(row);
                    }
                    //Insert data into table
                    var sqlQuery = "Insert into " + TableName + "(" + ColumnNames + ") Values(" + rowsStringBuilder + ");";
                    using (var cmd = new SQLiteCommand(sqlQuery, dbSqLiteConnection))
                        await cmd.ExecuteNonQueryAsync();
                }
                dbSqLiteConnection.Shutdown();
                dbSqLiteConnection.Close();
            }
            if (File.Exists(PathToDatabaseArchiveFile))
                File.Delete(PathToDatabaseArchiveFile);
            ZipFile.CreateFromDirectory(_pathToDatabase, PathToDatabaseArchiveFile);
        }

        private static string _pathToDatabase;
        private static string _pathToDatabaseFile;
        private static readonly string TableName = WebConfigurationManager.AppSettings.Get("DatabaseTableName");
        private static readonly string ColumnNames = WebConfigurationManager.AppSettings.Get("DatabaseColumnNames");

        private static readonly string PathToDatabaseArchiveFile = Consts.Consts.GetPath
            (WebConfigurationManager.AppSettings.Get("PathToDatabaseArchiveFile"));

        private static void SetPathToParentDirectoryOfDatabaseFile()
        {
            var builder =
                new SQLiteConnectionStringBuilder(WebConfigurationManager.ConnectionStrings["SQLite"].ConnectionString);
            _pathToDatabaseFile = Consts.Consts.GetPath(builder.DataSource);
            _pathToDatabase = Directory.GetParent(_pathToDatabaseFile).FullName + @"\";
        }
        private static async Task<DataSet> SetDataSet(string path)
        {
            var ds = new DataSet();
            var connectString = "";
            //connection strings for excel files from and rest code of this method based on
            //http://www.aspsnippets.com/Articles/Read-and-Import-Excel-File-into-DataSet-or-DataTable-using-C-and-VBNet-in-ASPNet.aspx
            if (Path.GetExtension(path).Equals(".xlsx"))
                connectString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1;\"";
            else if (Path.GetExtension(path).Equals(".xls"))
                connectString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Extended Properties=\"Excel 8.0 Xml;HDR=YES;IMEX=1;\"";
            using (var oleDbConnection = new OleDbConnection(connectString))
            {
                await oleDbConnection.OpenAsync();
                var dt = oleDbConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                if (dt == null)
                    return null;
                var excelSheets = new String[dt.Rows.Count];
                var t = 0;
                foreach (DataRow row in dt.Rows)
                {
                    excelSheets[t] = row["TABLE_NAME"].ToString();
                    t++;
                }
                var query = string.Format("Select * from [{0}]", excelSheets[0]);
                using (var dataAdapter = new OleDbDataAdapter(query, oleDbConnection))
                {
                    dataAdapter.Fill(ds);
                }
                oleDbConnection.Close();
            }
            return ds;
        }
    }
}