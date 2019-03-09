using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static AzureStorageDemo.Utils;

namespace AzureStorageDemo
{
    [TestClass]
    public class SQLBulkInsert
    {
        // Azure
        private const string connString = "";

        private DataTable CreateDataTable(int nRows)
        {
            var dataTable = new DataTable("Strings");

            var stringColumn = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "TheValue"
            };
            dataTable.Columns.Add(stringColumn);

            for (var i = 0; i < nRows; i++)
            {
                dataTable.Rows.Add(RandomString(1024));
            }

            return dataTable;
        }

        [TestMethod]
        public async Task BulkCopy()
        {
            const int rowsToInsert = 100000;
            var rowsToWrite = CreateDataTable(rowsToInsert);
            await Time($"Writing {rowsToInsert} rows via SqlBulkCopy",
                async () =>
                {
                    try
                    {
                        using (var sqlConn = new SqlConnection(connString))
                        using (var bulkCopy = new SqlBulkCopy(sqlConn)
                        {
                            BulkCopyTimeout = (int)TimeSpan.FromMinutes(1).TotalSeconds,
                            BatchSize = 1000
                        })
                        {
                            sqlConn.Open();
                            bulkCopy.DestinationTableName = "dbo.BulkCopyTable";
                            bulkCopy.ColumnMappings.Add("TheValue", "TheValue");
                            // Write from the source to the destination.
                            await bulkCopy.WriteToServerAsync(rowsToWrite);
                            bulkCopy.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteError($"Oops - {ex.Message}");
                    }
                });
        }
    }
}
