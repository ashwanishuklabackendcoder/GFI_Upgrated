using System;
using System.Data;
using Microsoft.Data.SqlClient;

class Program
{
    static void Main()
    {
        string connStr = "Server=db50414.public.databaseasp.net;Database=db50414;User Id=db50414;Password=m?2TQ9f#nZ+5;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;";
        try
        {
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (var cmd = new SqlCommand("W_ItemStockList", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@StockID", 0);
                    cmd.Parameters.AddWithValue("@CreatedBy", "");
                    cmd.Parameters.AddWithValue("@ItemID", 0);
                    cmd.Parameters.AddWithValue("@WarehouseID", 0);
                    cmd.Parameters.AddWithValue("@ItemTypeId", 0);
                    var pPage = cmd.Parameters.Add("@CurrentPage", SqlDbType.Int); pPage.Value = 1; pPage.Direction = ParameterDirection.InputOutput;
                    cmd.Parameters.AddWithValue("@RecordPerPage", 10);
                    var pTotal = cmd.Parameters.Add("@TotalRecord", SqlDbType.Int); pTotal.Direction = ParameterDirection.InputOutput;
                    cmd.Parameters.AddWithValue("@SortOrd", "DESC");
                    cmd.Parameters.AddWithValue("@SortColumn", "StockID");

                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        var table = new DataTable();
                        adapter.Fill(table);
                        Console.WriteLine("\n--- W_ItemStockList returned columns ---");
                        foreach (DataColumn col in table.Columns)
                        {
                            Console.WriteLine($"{col.ColumnName} ({col.DataType.Name})");
                        }

                        // Let's print the first row values if any
                        if (table.Rows.Count > 0)
                        {
                            Console.WriteLine("\n--- First Row Data ---");
                            DataRow row = table.Rows[0];
                            foreach (DataColumn col in table.Columns)
                            {
                                Console.WriteLine($"{col.ColumnName}: {row[col.ColumnName]}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("No records returned.");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
