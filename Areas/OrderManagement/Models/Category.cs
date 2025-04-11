using System.ComponentModel.DataAnnotations;

namespace Inventory_Management.Areas.OrderManagement.Models;

public class Category
{
    [Key]
    public int CategoryId { get; set; }

    [Required]
    [Display(Name = "Category Name")]
    [StringLength(100, ErrorMessage = "Category Name cannot be more than 100 characters.")]
    public string CategoryName { get; set; }

    [Display(Name = "Category Description")]
    [DataType(DataType.MultilineText)]
    [StringLength(500, ErrorMessage = "Category Description cannot be more than 500 characters.")]
    [DisplayFormat(NullDisplayText = "No Description Available")]
    public string? CategoryDescription { get; set; }

    public ICollection<Product>? Products { get; set; }

}