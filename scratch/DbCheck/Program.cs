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
                Console.WriteLine("Connected successfully!");

                string alterSql = @"
ALTER Procedure [dbo].[W_MasterItemTypeSelectMain]    
@ItemTypeId int=0    
as    
if(@ItemTypeId>0)    
select * from W_MasterItemType where ItemTypeId=@ItemTypeId    
else    
select * from W_MasterItemType where IsActive=1 and IsMainType=1
order by ItemTypeName
";
                using (var cmd = new SqlCommand(alterSql, conn))
                {
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Stored procedure W_MasterItemTypeSelectMain altered successfully to check IsMainType=1!");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
