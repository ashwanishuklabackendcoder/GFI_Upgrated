using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using GFI_Upgrated.Data.AdminSecurity;
using GFI_Upgrated.SharedDto.AdminSecurity;
using System.Threading.Tasks;
using System.Linq;
using System.Text.Json;

namespace DebugScript;

public class Program
{
    public static async Task Main()
    {
        var connectionString = "Server=db50414.public.databaseasp.net;Database=db50414;User Id=db50414;Password=m?2TQ9f#nZ+5;Encrypt=True;TrustServerCertificate=True;";
        // Note: AdminSecurityRepository needs IConfiguration or just the connection string in its base.
        // I'll manually check the columns from a DataTable first.
        
        using (var connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();
            using (var command = new SqlCommand("Z_UsersMenuList", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@RecordPerPage", 10);
                var totalParam = new SqlParameter("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };
                command.Parameters.Add(totalParam);
                command.Parameters.AddWithValue("@CurrentPage", 1);
                command.Parameters.AddWithValue("@SortColumn", "LinkID");
                command.Parameters.AddWithValue("@SortOrd", "ASC");

                var table = new DataTable();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    table.Load(reader);
                }

                Console.WriteLine("Columns in DataTable:");
                foreach (DataColumn col in table.Columns)
                {
                    Console.WriteLine($"- {col.ColumnName}");
                }

                if (table.Rows.Count > 0)
                {
                    var row = table.Rows[0];
                    Console.WriteLine("\nFirst Row Data:");
                    Console.WriteLine($"PageHeading: {row["PageHeading"]}");
                    Console.WriteLine($"DisplayName: {row["DisplayName"]}");
                    Console.WriteLine($"IconClass: {row["IconClass"]}");
                }
            }
        }
    }
}
