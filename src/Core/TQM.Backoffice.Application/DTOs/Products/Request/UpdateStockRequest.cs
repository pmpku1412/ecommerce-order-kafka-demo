using System.ComponentModel.DataAnnotations;

namespace TQM.Backoffice.Core.Application.DTOs.Products.Request;

public class UpdateStockRequest
{
    [Required(ErrorMessage = "Product ID is required")]
    public string ProductId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Stock is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
    public int Stock { get; set; }

    [StringLength(200, ErrorMessage = "Reason cannot exceed 200 characters")]
    public string Reason { get; set; } = string.Empty;
}
