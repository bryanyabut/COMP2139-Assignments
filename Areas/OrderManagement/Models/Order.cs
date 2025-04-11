using System.ComponentModel.DataAnnotations;

namespace Inventory_Management.Areas.OrderManagement.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        [Display(Name = "Order Number")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime OrderDate { get; set; }

        [Required]
        public decimal TotalPrice { get; set; }
        
        [Required]
        public string GuestName { get; set; }
        [Required]
        public string GuestEmail { get; set; }

        public ICollection<OrderProduct>? OrderProducts { get; set; }
    }
}
