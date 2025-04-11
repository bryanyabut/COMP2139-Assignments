using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory_Management.Areas.OrderManagement.Models
{
    public class OrderProduct
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order? Orders { get; set; }

        public int ProductId { get; set; }
        public Product? Products { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}
