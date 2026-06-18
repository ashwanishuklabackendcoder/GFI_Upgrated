using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GFI_Upgrated.SharedDto.Store;

public class KettleIdConverter : JsonConverter<string?>
{
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.GetInt64().ToString();
        }
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }
        return reader.GetString();
    }

    public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
    {
        if (value == null)
            writer.WriteNullValue();
        else
            writer.WriteStringValue(value);
    }
}

public sealed class ProductionDto
{
    public long ProductionId { get; set; }
    public long BomId { get; set; }
    public string? BomName { get; set; }
    public long BomQty { get; set; }
    
    public string? CookingDate { get; set; }
    public string? ExpiryDate { get; set; }
    public string? FilledDate { get; set; }
    
    public string? PackedCountryName { get; set; }
    public string? PackedCountry { get; set; }
    public string? Colli { get; set; }
    public string? PalletNumber { get; set; }
    public string? ProcessEmployees { get; set; }
    
    [JsonConverter(typeof(KettleIdConverter))]
    public string? KettleId { get; set; }
    public string? KettleNumber { get; set; }
    public int KettleRun { get; set; }
    
    public string? BatchNo { get; set; }
    public long SkuId { get; set; }
    public string? SkuName { get; set; }
    public string? DocumentUpload { get; set; }
    
    public int FillingBottles { get; set; }
    public long FillingPerBottleUnit { get; set; }
    public string? FillingUnitName { get; set; }
    public double ExtraBottles { get; set; }
    public long WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    
    public string? Remarks { get; set; }
    public int IsComplete { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
}

public sealed class SaveProductionRequest
{
    public long ProductionId { get; set; }
    
    [Required(ErrorMessage = "BOM is required")]
    public long BomId { get; set; }
    
    [Range(1, long.MaxValue, ErrorMessage = "BOM Quantity must be at least 1")]
    public long BomQty { get; set; }
    
    [Required(ErrorMessage = "Cooking Date is required")]
    public DateTime? CookingDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DateTime? FilledDate { get; set; }
    
    public string? PackedCountry { get; set; }
    public string? Colli { get; set; }
    public string? PalletNumber { get; set; }
    public string? ProcessEmployees { get; set; }
    
    [JsonConverter(typeof(KettleIdConverter))]
    public string? KettleId { get; set; }
    public int KettleRun { get; set; }
    
    public string? BatchNo { get; set; }
    public long SkuId { get; set; }
    public string? DocumentUpload { get; set; }
    
    public int FillingBottles { get; set; }
    public long FillingPerBottleUnit { get; set; }
    public double ExtraBottles { get; set; }
    public long WarehouseId { get; set; }
    
    public string? Remarks { get; set; }
    public string UpdatedBy { get; set; } = "System";
}

public sealed class CountryLookupDto
{
    public long CountryId { get; set; }
    public string? CountryName { get; set; }
}

public sealed class SkuLookupDto
{
    public long SkuId { get; set; }
    public string? SkuName { get; set; }
}

public sealed class KettleLookupDto
{
    public long KettleId { get; set; }
    public string? KettleNumber { get; set; }
}
