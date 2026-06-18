using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace DebugScript;

public class Program
{
    public static async Task Main()
    {
        var connectionString = "Server=db50414.public.databaseasp.net;Database=db50414;User Id=db50414;Password=m?2TQ9f#nZ+5;Encrypt=True;TrustServerCertificate=True;";
        
        using (var connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();
            using (var command = new SqlCommand("SELECT PARAMETER_NAME FROM INFORMATION_SCHEMA.PARAMETERS WHERE SPECIFIC_NAME = 'W_PurchaseChildModify'", connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    Console.WriteLine("Parameters for W_PurchaseChildModify:");
                    while (await reader.ReadAsync())
                    {
                        Console.WriteLine($"- {reader["PARAMETER_NAME"]}");
                    }
                }
            }
            
            using (var command = new SqlCommand("SELECT OBJECT_DEFINITION(OBJECT_ID('W_PurchaseChildModify'))", connection))
            {
                var definition = await command.ExecuteScalarAsync();
                Console.WriteLine("\nDefinition of W_PurchaseChildModify:");
                Console.WriteLine(definition);
            }
        }
    }
}
