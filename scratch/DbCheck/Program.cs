using System;
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
                using (var cmd = new SqlCommand("SELECT definition FROM sys.sql_modules WHERE object_id = OBJECT_ID('Z_UsersLoginsModify')", conn))
                {
                    var def = cmd.ExecuteScalar()?.ToString();
                    Console.WriteLine("--- Z_UsersLoginsModify Definition ---");
                    Console.WriteLine(def ?? "Not found");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
