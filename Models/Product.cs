using System.ComponentModel.DataAnnotations;

namespace Inventory_Management.Models;

public class Product
{
    // The key attribute is used to specify the primary key of the table
    [Key]
    public int ProductId { get; set; }

    [Required]
    public required string ProductName { get; set; }
    
    public string? ProductDescription { get; set; }

    [Required]
    public decimal ProductPrice { get; set; }

    [Required]
    public int ProductQuantity { get; set; }

    [Required]
    public int LowStockThreshold { get; set; }

    public int CategoryId { get; set; }
    public Category? Categories { get; set; }

    public ICollection<OrderProduct>? OrderProducts { get; set; }
}