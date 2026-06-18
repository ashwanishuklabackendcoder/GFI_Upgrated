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
                Console.WriteLine("--- TABLES ---");
                using (SqlCommand cmd = new SqlCommand("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_NAME", connection))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read()) Console.WriteLine(reader["TABLE_NAME"]);
                }
                
                Console.WriteLine("\n--- PROCEDURES ---");
                using (SqlCommand cmd = new SqlCommand("SELECT ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_TYPE = 'PROCEDURE' ORDER BY ROUTINE_NAME", connection))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read()) Console.WriteLine(reader["ROUTINE_NAME"]);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
