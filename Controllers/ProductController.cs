using Inventory_Management.data;
using Inventory_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Inventory_Management.Controllers
{
    public class ProductController : Controller
    {
        private readonly InventoryDbContext _context;
        // Add a logger to the controller
        // used to log information, warnings, and errors
        private readonly ILogger<ProductController> _logger;

        public ProductController(InventoryDbContext context, ILogger<ProductController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        // GET: ProductController
        public async Task<IActionResult> Index(string search, decimal? minPrice, decimal? maxPrice, string sortOrder)
        {
            // get product from database
            // include the category of the product as it is a one-to-many relationship
            var products = await _context.Products.Include(p => p.Categories).ToListAsync();
            // filter products based on search, minPrice, and maxPrice
            if (!string.IsNullOrWhiteSpace(search))
            {
                products = products.Where(p => p.ProductName.Contains(search)).ToList();
            }
            if (minPrice != null)
            {
                products = products.Where(p => p.ProductPrice >= minPrice).ToList();
            }
            if (maxPrice != null)
            {
                products = products.Where(p => p.ProductPrice <= maxPrice).ToList();
            }

            // sort products based on sortOrder
            if (sortOrder == "descending-name")
            {
                products = products.OrderByDescending(p => p.ProductName).ToList();
            }
            else if (sortOrder == "ascending-price")
            {
                products = products.OrderBy(p => p.ProductPrice).ToList();
            }
            else if (sortOrder == "descending-price")
            {
                products = products.OrderByDescending(p => p.ProductPrice).ToList();
            }
            else
            {
                products = products.OrderBy(p => p.ProductName).ToList();
            }
            var lowStockProducts = products.Where(p => p.ProductQuantity <= p.LowStockThreshold).ToList();
            ViewBag.LowStockProducts = lowStockProducts;
            return View(products);
        }

        [HttpGet]
        // GET: ProductController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            // get product from database
            // get product with the specified id or return null if not found
            // include the category of the product as it is a one to many relationship
            var product = await _context.Products.Include(p => p.Categories).FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpGet]
        // GET: ProductController/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "CategoryId", "CategoryName");
            return View();
        }

        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            // [ValidateAntiForgeryToken] helps prevent CSRF attacks
            // Check if the model is valid
            _logger.LogInformation("Create action called");
            if (ModelState.IsValid)
            {
                // Add product to the database
                _context.Add(product);
                _logger.LogInformation("Check if product is added to context");
                await _context.SaveChangesAsync();
                _logger.LogInformation("check if product is saved to database");
                return RedirectToAction(nameof(Index));
            }
            else
            {
                // Log the ModelState errors
                _logger.LogWarning("ModelState is not valid");
                // Log the ModelState errors
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        _logger.LogWarning($"Property: {state.Key}, Error: {error.ErrorMessage}");
                    }
                }
            }
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        [HttpGet]
        // GET: ProductController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // POST: ProductController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,ProductDescription,ProductPrice,ProductQuantity,LowStockThreshold,CategoryId")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        private bool ProductExists(int productId)
        {
            return _context.Products.Any(p => p.ProductId == productId);
        }

        [HttpGet]
        // GET: ProductController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.Include(p => p.Categories).FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: ProductController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}





