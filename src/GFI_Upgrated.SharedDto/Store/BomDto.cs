using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GFI_Upgrated.SharedDto.Store;

public class BomDto
{
    public long BomId { get; set; }
    public long ItemId { get; set; }
    public string? ItemName { get; set; }
    public string? BomName { get; set; }
    public int Quantity { get; set; }
    public long UnitId { get; set; }
    public string? UnitName { get; set; }
    public double ExtraExpensesPerPiece { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public bool IsActive { get; set; }
    public long ItemTypeId { get; set; }
}

public class BomItemDto
{
    public long BomItemsId { get; set; }
    public long BomId { get; set; }
    public long ItemID { get; set; }
    public string? ItemName { get; set; }
    public double Quantity { get; set; }
    public long UnitId { get; set; }
    public string? UnitName { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
}

public class BomListRequest
{
    public string? SearchTerm { get; set; }
    public int? ItemTypeId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortColumn { get; set; }
    public string? SortType { get; set; } // ASC or DESC
}

public class SaveBomRequest
{
    public long BomId { get; set; }
    
    [Required(ErrorMessage = "Parent product is required")]
    public long ItemId { get; set; }
    
    [Required(ErrorMessage = "BOM Name is required")]
    public string? BomName { get; set; }
    
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; }
    
    [Required(ErrorMessage = "Unit is required")]
    public long UnitId { get; set; }
    
    public double ExtraExpensesPerPiece { get; set; }
    public string? CreatedBy { get; set; }
    public bool IsActive { get; set; } = true;
    public long ItemTypeId { get; set; }
    
    // Components list for batch save
    public List<BomItemDto> Items { get; set; } = new();
}
