using System;
using System.Data;
using Microsoft.Data.SqlClient;

class Program
{
    static void Main()
    {
        string connectionString = "Server=db50414.public.databaseasp.net;Database=db50414;User Id=db50414;Password=m?2TQ9f#nZ+5;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;";
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                Console.WriteLine("Connection successful.");
                
                DataTable schema = connection.GetSchema("Columns", new string[] { null, null, "W_MasterBrands" });
                foreach (DataRow row in schema.Rows)
                {
                    Console.WriteLine($"Column: {row["COLUMN_NAME"]}, Type: {row["DATA_TYPE"]}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
