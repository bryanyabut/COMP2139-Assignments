using Inventory_Management.data;
using Inventory_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Management.Controllers
{
    public class OrderController : Controller
    {
        private readonly InventoryDbContext _context;

        public OrderController(InventoryDbContext context)
        {
            _context = context;
        }

        // GET: OrderController
        public IActionResult Index()
        {
            var orders = _context.Orders
                .Include(o => o.OrderProducts)
                .ThenInclude(p => p.Products)
                .ToList();
            return View(orders);
        }

        // GET: OrderController/Details/5
        public IActionResult Details(int id)
        {
            var orders = _context.Orders
                .Include(o => o.OrderProducts)
                .ThenInclude(p => p.Products)
                .FirstOrDefault(o => o.OrderId == id);
            if (orders == null)
            {
                return NotFound();
            }

            return View(orders);
        }

        // GET: OrderController/Create
        public IActionResult Create()
        {
            ViewBag.Products = new SelectList(_context.Products, "ProductId", "ProductName");
            return View();
        }

        // POST: OrderController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Order order, int[] productIds, int[] quantities)
        {
            // [ValidateAntiForgeryToken] helps prevent CSRF attacks
            // Check if the model is valid
            if (ModelState.IsValid)
            {
                order.OrderDate = DateTime.UtcNow; // Set the order date to the current date
                order.TotalPrice = 0; // Initialize the total price to 0
                _context.Orders.Add(order); // Add the order to the database
                _context.SaveChanges(); // Save changes to the database to get the order id

                // Create a linked list to store the order products
                var orderProducts = new LinkedList<OrderProduct>();

                // Loop through the product ids and quantities
                for (int i = 0; i < productIds.Length; i++)
                {
                    // Find the product with the specified id
                    var product = _context.Products.Find(productIds[i]);
                    if (product != null)
                    {
                        // Create a new order product
                        var orderProduct = new OrderProduct
                        {
                            // Set the current order id, product id, and quantity
                            OrderId = order.OrderId,
                            ProductId = productIds[i],
                            Quantity = quantities[i]
                        };
                        // Calculate the total price of the order
                        order.TotalPrice += product.ProductPrice * quantities[i];
                        // Update the product quantity
                        product.ProductQuantity -= quantities[i];
                        // Add the order product to the linked list
                        orderProducts.AddLast(orderProduct);
                    }
                }

                // Iterates through the linked list of order products
                foreach (var orderProduct in orderProducts)
                {
                    _context.OrdersProducts.Add(orderProduct);
                }

                // Save changes to the database
                _context.SaveChanges();
                // Redirect to the index page
                return RedirectToAction("Index");
            }

            // Set the view bag to the list of products
            ViewBag.Products = new SelectList(_context.Products, "ProductId", "ProductName");
            // Return the view with the order
            return View(order);
        }

        // GET: OrderController/Delete/5
        public IActionResult Delete(int id)
        {
            // Find the order with the specified id
            var order = _context.Orders
                .Include(op => op.OrderProducts)
                .ThenInclude(p => p.Products)
                .FirstOrDefault(o => o.OrderId == id);
            // Check if the order is null
            if (order == null)
            {
                return NotFound();
            }

            // Return the view with the order
            return View(order);
        }

        // POST: OrderController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var order = _context.Orders
                .Include(o => o.OrderProducts)
                .FirstOrDefault(o => o.OrderId == id);

            if (order != null)
            {
                // Remove associated OrderProducts
                var orderProducts = _context.OrdersProducts
                    .Where(op => op.OrderId == id);
                _context.OrdersProducts.RemoveRange(orderProducts);

                // Remove the order
                _context.Orders.Remove(order);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}