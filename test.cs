using System;
using System.Data.SqlClient;

class Program {
    static void Main() {
        string connStr = ""Server=db50414.public.databaseasp.net;Database=db50414;User Id=db50414;Password=m?2TQ9f#nZ+5;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"";
        using (SqlConnection conn = new SqlConnection(connStr)) {
            conn.Open();
            SqlCommand cmd = new SqlCommand(""sp_helptext 'W_ItemStockList'"", conn);
            using (SqlDataReader reader = cmd.ExecuteReader()) {
                while (reader.Read()) {
                    Console.Write(reader[0]);
                }
            }
        }
    }
}
