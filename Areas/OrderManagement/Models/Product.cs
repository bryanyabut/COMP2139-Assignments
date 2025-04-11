using System.ComponentModel.DataAnnotations;

namespace Inventory_Management.Areas.OrderManagement.Models;

public class Product
{
    // The key attribute is used to specify the primary key of the table
    [Key]
    public int ProductId { get; set; }

    [Display(Name = "Product Name")]
    [Required]
    [StringLength(100, ErrorMessage = "Product Name cannot be more than 100 characters.")]
    public required string ProductName { get; set; }
    
    [Display(Name = "Product Description")]
    [DataType(DataType.MultilineText)]
    [StringLength(500, ErrorMessage = "Product Description cannot be more than 500 characters.")]
    public string? ProductDescription { get; set; }
    
    [Required]
    [Display(Name = "Product Price")]
    [DataType(DataType.Currency)]
    public decimal ProductPrice { get; set; }

    [Required]
    [Display(Name = "Product Quantity")]
    [Range(0, int.MaxValue, ErrorMessage = "Product Quantity must be a positive number.")]
    public int ProductQuantity { get; set; }

    [Required]
    [Display(Name = "Low Stock Threshold")]
    [Range(0, int.MaxValue, ErrorMessage = "Low Stock Threshold must be a positive number.")]
    public int LowStockThreshold { get; set; }

    public int CategoryId { get; set; }
    public Category? Categories { get; set; }

    public ICollection<OrderProduct>? OrderProducts { get; set; }
}