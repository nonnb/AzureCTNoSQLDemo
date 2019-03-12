using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using static AzureStorageDemo.Utils;

namespace AzureStorageDemo
{
    public class SqlBulkInsert
    {
        public static async Task<TimeSpan> BulkCopy(int rowsToInsert)
        {
            var rowsToWrite = CreateDataTable(rowsToInsert);
            return await Time(
                async () =>
                {
                    using (var sqlConn = new SqlConnection(ConnString))
                    using (var bulkCopy = new SqlBulkCopy(sqlConn)
                    {
                        BulkCopyTimeout = (int) TimeSpan.FromMinutes(1).TotalSeconds,
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
                });
        }

        // Azure
        private const string ConnString = "{yourconnectionstringhere}";
        // Local on Prem
        // private const string connString = @"Data Source=.\SQLEXPRESS;Initial Catalog=AzureCT;Integrated Security=True";

        private static DataTable CreateDataTable(int nRows)
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
    }
}
