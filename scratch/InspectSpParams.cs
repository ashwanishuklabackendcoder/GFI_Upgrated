using System;
using System.Data;
using Microsoft.Data.SqlClient;

class Program
{
    static void Main()
    {
        string connectionString = "Server=db50414.public.databaseasp.net;Database=db50414;User Id=db50414;Password=m?2TQ9f#nZ+5;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;";
        string[] procedures = { "W_MasterBrandsList", "W_MasterBrandsModify", "W_MasterBrandsOperation", "W_MasterBrandsSelectAll" };
        
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                foreach (var sp in procedures)
                {
                    Console.WriteLine($"\n--- Parameters for {sp} ---");
                    using (SqlCommand cmd = new SqlCommand("SELECT PARAMETER_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, PARAMETER_MODE FROM INFORMATION_SCHEMA.PARAMETERS WHERE SPECIFIC_NAME = @sp", connection))
                    {
                        cmd.Parameters.AddWithValue("@sp", sp);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine($"{reader["PARAMETER_NAME"]} ({reader["DATA_TYPE"]} {reader["CHARACTER_MAXIMUM_LENGTH"]}): {reader["PARAMETER_MODE"]}");
                            }
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
