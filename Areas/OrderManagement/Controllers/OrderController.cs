using Inventory_Management.data;
using Inventory_Management.Models;
using Inventory_Management.Areas.OrderManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Management.Areas.OrderManagement.Controllers
{
    [Area("OrderManagement")]
    [Route("[area]/[controller]/[action]")]
    public class OrderController : Controller
    {
        private readonly InventoryDbContext _context;
        private readonly ILogger<OrderController> _logger;

        public OrderController(InventoryDbContext context, ILogger<OrderController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: OrderController
        [HttpGet]
        [Route("")]
        [Route("/OrderManagement/Order")]
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Fetching all orders.");
            var orders = await _context.Orders
                .Include(o => o.OrderProducts)
                .ThenInclude(p => p.Products)
                .ToListAsync();
            return View(orders);
        }

        // GET: OrderController/Details/5
        [HttpGet]
        [Route("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation("Fetching details for order with ID {OrderId}.", id);
            var orders = await _context.Orders
                .Include(o => o.OrderProducts)
                .ThenInclude(p => p.Products)
                .FirstOrDefaultAsync(o => o.OrderId == id);
            if (orders == null)
            {
                _logger.LogWarning("Order with ID {OrderId} not found.", id);
                return NotFound();
            }

            return View(orders);
        }

        // GET: OrderController/Create
        [HttpGet]
        [Route("Create")]
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation("Navigating to Create Order view.");
            ViewBag.Products = new SelectList(await _context.Products.ToListAsync(), "ProductId", "ProductName");
            return View();
        }

        // POST: OrderController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public async Task<IActionResult> Create(Order order, int[] productIds, int[] quantities)
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Creating a new order for guest {GuestName}.", order.GuestName);
                try
                {
                    order.OrderDate = DateTime.UtcNow;
                    order.TotalPrice = 0;
                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync();

                    var orderProducts = new LinkedList<OrderProduct>();
                    for (int i = 0; i < productIds.Length; i++)
                    {
                        var product = await _context.Products.FindAsync(productIds[i]);
                        if (product != null)
                        {
                            var orderProduct = new OrderProduct
                            {
                                OrderId = order.OrderId,
                                ProductId = productIds[i],
                                Quantity = quantities[i]
                            };
                            order.TotalPrice += product.ProductPrice * quantities[i];
                            product.ProductQuantity -= quantities[i];
                            orderProducts.AddLast(orderProduct);
                        }
                    }

                    foreach (var orderProduct in orderProducts)
                    {
                        _context.OrdersProducts.Add(orderProduct);
                    }

                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Order for guest {GuestName} created successfully.", order.GuestName);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while creating the order for guest {GuestName}.",
                        order.GuestName);
                    throw;
                }
            }

            _logger.LogWarning("Failed to create order due to invalid model state.");
            ViewBag.Products = new SelectList(await _context.Products.ToListAsync(), "ProductId", "ProductName");
            return View(order);
        }

        // GET: OrderController/Delete/5
        [HttpGet]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Fetching order with ID {OrderId} for deletion.", id);
            var order = await _context.Orders
                .Include(op => op.OrderProducts)
                .ThenInclude(p => p.Products)
                .FirstOrDefaultAsync(o => o.OrderId == id);
            if (order == null)
            {
                _logger.LogWarning("Order with ID {OrderId} not found for deletion.", id);
                return NotFound();
            }

            return View(order);
        }

        // POST: OrderController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Delete/{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _logger.LogInformation("Deleting order with ID {OrderId}.", id);
            var order = await _context.Orders
                .Include(o => o.OrderProducts)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order != null)
            {
                try
                {
                    var orderProducts = _context.OrdersProducts
                        .Where(op => op.OrderId == id);
                    _context.OrdersProducts.RemoveRange(orderProducts);

                    _context.Orders.Remove(order);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Order with ID {OrderId} deleted successfully.", id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while deleting the order with ID {OrderId}.", id);
                    throw;
                }
            }
            else
            {
                _logger.LogWarning("Order with ID {OrderId} not found during deletion.", id);
            }

            return RedirectToAction("Index");
        }
    }
}