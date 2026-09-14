using System;
using Microsoft.Data.SqlClient;

class Program
{
    static void Main()
    {
        string connectionString = "Server=db50414.public.databaseasp.net;Database=db50414;User Id=db50414;Password=m?2TQ9f#nZ+5;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;";
        
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();

            Console.WriteLine("=================== SEARCHING FOR ALL ITEMS WITH NEGATIVE ISSUED QUANTITY ===================");
            
            string qNegIssued = @"
                SELECT 
                    s.StockID,
                    s.ItemID,
                    mi.ItemName,
                    ROUND(s.OpeningQuantity, 2) AS OpeningQty,
                    ROUND(s.PurchasedQuantity, 2) AS PurchasedQty,
                    ROUND(s.ProducedQuantity, 2) AS ProducedQty,
                    ROUND(s.IssuedQuantity, 2) AS IssuedQty,
                    ROUND(s.RemovedQuantity, 2) AS RemovedQty,
                    ROUND(s.FinalStock, 2) AS FinalStock,
                    ROUND((s.OpeningQuantity + s.PurchasedQuantity + s.ProducedQuantity - s.RemovedQuantity) - s.FinalStock, 2) AS ExpectedIssuedQty
                FROM W_ItemStock s
                INNER JOIN W_MasterItem mi ON s.ItemID = mi.ItemID
                WHERE s.IssuedQuantity < 0 OR (s.OpeningQuantity + s.PurchasedQuantity + s.ProducedQuantity - s.RemovedQuantity - s.FinalStock) < 0
                ORDER BY s.IssuedQuantity ASC;";

            using (SqlCommand cmd = new SqlCommand(qNegIssued, conn))
            using (SqlDataReader r = cmd.ExecuteReader())
            {
                int count = 0;
                while (r.Read())
                {
                    count++;
                    Console.WriteLine($"#{count} ItemID: {r["ItemID"]} ({r["ItemName"]}) | IssuedQty: {r["IssuedQty"]} | ProducedQty: {r["ProducedQty"]} | FinalStock: {r["FinalStock"]} | ExpectedIssuedQty: {r["ExpectedIssuedQty"]}");
                }
                if (count == 0)
                {
                    Console.WriteLine("None! No other items have negative IssuedQuantity in W_ItemStock.");
                }
            }
        }
    }
}
