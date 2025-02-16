using System.ComponentModel.DataAnnotations;

namespace Inventory_Management.Models;

public class Category
{
    [Key]
    public int CategoryId { get; set; }

    [Required]
    public string CategoryName { get; set; }

    public string? CategoryDescription { get; set; }

    public ICollection<Product>? Products { get; set; }

}