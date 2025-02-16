using System.ComponentModel.DataAnnotations;

namespace Inventory_Management.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        [DataType(DataType.Date)]
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
