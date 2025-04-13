using Inventory_Management.data;
using Inventory_Management.Models;
using Inventory_Management.Areas.OrderManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Inventory_Management.Areas.OrderManagement.Controllers
{
    [Area("OrderManagement")]
    [Route("[area]/[controller]/[action]")]
    [Authorize]
    public class ProductController : Controller
    {
        private readonly InventoryDbContext _context;
        private readonly ILogger<ProductController> _logger;

        public ProductController(InventoryDbContext context, ILogger<ProductController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Search(string query, int? categoryId)
        {
            _logger.LogInformation("Search action called with query: {Query} and categoryId: {CategoryId}", query,
                categoryId);

            var products = _context.Products.Include(p => p.Categories)
                .Where(p => (string.IsNullOrEmpty(query) || p.ProductName.Contains(query)) &&
                            (!categoryId.HasValue || p.CategoryId == categoryId))
                .ToList();

            _logger.LogInformation("Search action returned {ProductCount} products.", products.Count);
            return PartialView("_ProductListPartial", products);
        }

        [HttpPost]
        public IActionResult AddProduct(Product product)
        {
            _logger.LogInformation("AddProduct action called for product: {ProductName}", product.ProductName);

            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                _context.SaveChanges();
                _logger.LogInformation("Product {ProductName} added successfully.", product.ProductName);
                return PartialView("_ProductListPartial", _context.Products.Include(p => p.Categories).ToList());
            }

            _logger.LogWarning("AddProduct action failed due to invalid model state.");
            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    _logger.LogWarning("Validation error for {Property}: {ErrorMessage}", state.Key,
                        error.ErrorMessage);
                }
            }

            return BadRequest("Invalid product data.");
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index(string search, decimal? minPrice, decimal? maxPrice, string sortOrder)
        {
            _logger.LogInformation(
                "Index action called with search: {Search}, minPrice: {MinPrice}, maxPrice: {MaxPrice}, sortOrder: {SortOrder}",
                search, minPrice, maxPrice, sortOrder);

            var products = await _context.Products.Include(p => p.Categories).ToListAsync();

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
            ViewBag.Categories = await _context.Categories.ToListAsync();

            _logger.LogInformation("Index action returned {ProductCount} products.", products.Count);
            return View(products);
        }

        [HttpGet]
        [Route("Details/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation("Details action called for product ID: {ProductId}", id);

            var product = await _context.Products.Include(p => p.Categories)
                .FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found.", id);
                return NotFound();
            }

            _logger.LogInformation("Details action returned product: {ProductName}", product.ProductName);
            return View(product);
        }

        [HttpGet]
        [Route("Create")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation("Create GET action called.");
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "CategoryId", "CategoryName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<IActionResult> Create(Product product)
        {
            _logger.LogInformation("Create POST action called for product: {ProductName}", product.ProductName);

            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Product {ProductName} created successfully.", product.ProductName);
                return RedirectToAction(nameof(Index));
            }

            _logger.LogWarning("Create POST action failed due to invalid model state.");
            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    _logger.LogWarning("Validation error for {Property}: {ErrorMessage}", state.Key,
                        error.ErrorMessage);
                }
            }

            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "CategoryId", "CategoryName",
                product.CategoryId);
            return View(product);
        }

        [HttpGet]
        [Route("Edit/{id:int}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation("Edit GET action called for product ID: {ProductId}", id);

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found.", id);
                return NotFound();
            }

            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "CategoryId", "CategoryName",
                product.CategoryId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit/{id:int}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<IActionResult> Edit(int id,
            [Bind("ProductId,ProductName,ProductDescription,ProductPrice,ProductQuantity,LowStockThreshold,CategoryId")]
            Product product)
        {
            _logger.LogInformation("Edit POST action called for product ID: {ProductId}", id);

            if (id != product.ProductId)
            {
                _logger.LogWarning("Edit POST action failed due to mismatched product ID.");
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Product with ID {ProductId} updated successfully.", id);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        _logger.LogWarning(
                            "Edit POST action failed because product with ID {ProductId} does not exist.", id);
                        return NotFound();
                    }
                    else
                    {
                        _logger.LogError(
                            "Edit POST action encountered a concurrency exception for product ID {ProductId}.", id);
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            _logger.LogWarning("Edit POST action failed due to invalid model state.");
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "CategoryId", "CategoryName",
                product.CategoryId);
            return View(product);
        }

        [HttpGet]
        [Route("Delete/{id:int}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Delete GET action called for product ID: {ProductId}", id);

            var product = await _context.Products.Include(p => p.Categories)
                .FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found.", id);
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Delete/{id:int}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _logger.LogInformation("Delete POST action called for product ID: {ProductId}", id);

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Delete POST action failed because product with ID {ProductId} does not exist.", id);
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Product with ID {ProductId} deleted successfully.", id);
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int productId)
        {
            var exists = _context.Products.Any(p => p.ProductId == productId);
            _logger.LogInformation("ProductExists check for product ID {ProductId}: {Exists}", productId, exists);
            return exists;
        }
    }
}