using System;
using System.Collections.Generic;

namespace GFI_Upgrated.SharedDto.Store;

public class RawMaterialDto
{
    public long ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string ItemCode { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public long ItemCatId { get; set; }
    public string ItemCategoryName { get; set; } = string.Empty;
    public long ItemTypeId { get; set; }
    public string ItemTypeName { get; set; } = string.Empty;
    public long PurchaseUnit { get; set; }
    public string UnitName { get; set; } = string.Empty;
    public int StatusId { get; set; }
    public bool IsActive { get; set; }
    public string? Description { get; set; }
    public string? StorageDetails { get; set; }
    public string? Tags { get; set; }
    public int TentativeExpiryDays { get; set; }
    public long BrandId { get; set; }
    public string? CreatedBy { get; set; }
}

public class RawMaterialListRequest
{
    public string? SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortColumn { get; set; }
    public string? SortType { get; set; }
}

public class SaveRawMaterialRequest
{
    public long ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string ItemCode { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public long ItemCatId { get; set; }
    public long ItemTypeId { get; set; } = 3; // Default to Raw Material type
    public int StatusId { get; set; } = 1;
    public string? Description { get; set; }
    public string? StorageDetails { get; set; }
    public string? Tags { get; set; }
    public int TentativeExpiryDays { get; set; }
    public long PurchaseUnit { get; set; }
    public long BrandId { get; set; }
    public string? CreatedBy { get; set; }
}

public class RawMaterialDetailDto
{
    public long ItemDetailId { get; set; }
    public long ItemId { get; set; }
    public bool CriticalLevelAlert { get; set; }
    public int CriticalLevelQuantity { get; set; }
    public int ReorderLevelQuantity { get; set; }
    public int ReorderLevelDays { get; set; }
    public int MaximumQuantity { get; set; }
    public double OpeningQuantity { get; set; }
    public double CurrentQuantity { get; set; }
}

public class RawMaterialVendorDto
{
    public long ItemVendorId { get; set; }
    public long ItemId { get; set; }
    public int VendorAccountId { get; set; }
    public string? VendorName { get; set; }
    public int StatusId { get; set; }
    public double PurchasePrice { get; set; }
    public decimal PurchaseUnit { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Remarks { get; set; }
    public long BrandId { get; set; }
    public long CurrencyId { get; set; }
    public double PurchaseQuantity { get; set; }
    public string? CreatedBy { get; set; }
}

public class RawMaterialBatchDto
{
    public long ItemStockByBatchId { get; set; }
    public long PurchaseID { get; set; }
    public long ItemId { get; set; }
    public int WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    public double Quantity { get; set; }
    public int UnitId { get; set; }
    public string? UnitName { get; set; }
    public string? BatchNo { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? CreatedBy { get; set; }
}
