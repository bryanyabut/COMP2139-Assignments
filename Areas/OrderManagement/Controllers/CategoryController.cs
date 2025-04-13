using Inventory_Management.data;
using Inventory_Management.Models;
using Inventory_Management.Areas.OrderManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Inventory_Management.Areas.OrderManagement.Controllers
{
    [Area("OrderManagement")]
    [Route("[area]/[controller]/[action]")]
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly InventoryDbContext _context;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(InventoryDbContext context, ILogger<CategoryController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: CategoryController
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Fetching all categories.");
            var categories = await _context.Categories.ToListAsync();
            return View(categories);
        }

        // GET: CategoryController/Details/5
        [HttpGet]
        [Route("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation("Fetching details for category with ID {CategoryId}.", id);
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
            if (category == null)
            {
                _logger.LogWarning("Category with ID {CategoryId} not found.", id);
                return NotFound();
            }

            return View(category);
        }

        // GET: CategoryController/Create
        [HttpGet]
        [Route("Create")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public IActionResult Create()
        {
            _logger.LogInformation("Navigating to Create Category view.");
            return View();
        }

        // POST: CategoryController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Creating a new category: {CategoryName}.", category.CategoryName);
                _context.Add(category);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Category {CategoryName} created successfully.", category.CategoryName);
                return RedirectToAction("Index");
            }

            _logger.LogWarning("Failed to create category due to invalid model state.");
            return View(category);
        }

        // GET: CategoryController/Edit/5
        [HttpGet]
        [Route("Edit/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation("Fetching category with ID {CategoryId} for editing.", id);
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                _logger.LogWarning("Category with ID {CategoryId} not found for editing.", id);
                return NotFound();
            }

            return View(category);
        }

        // POST: CategoryController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<IActionResult> Edit(int id,
            [Bind("CategoryId, CategoryName, CategoryDescription")] Category category)
        {
            if (id != category.CategoryId)
            {
                _logger.LogWarning("Category ID mismatch during edit. Provided ID: {ProvidedId}, Model ID: {ModelId}.",
                    id, category.CategoryId);
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _logger.LogInformation("Updating category with ID {CategoryId}.", id);
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Category with ID {CategoryId} updated successfully.", id);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await CategoryExists(category.CategoryId))
                    {
                        _logger.LogWarning("Category with ID {CategoryId} does not exist during update.", id);
                        return NotFound();
                    }
                    else
                    {
                        _logger.LogError("Concurrency error occurred while updating category with ID {CategoryId}.",
                            id);
                        throw;
                    }
                }

                return RedirectToAction("Index");
            }

            _logger.LogWarning("Failed to update category with ID {CategoryId} due to invalid model state.", id);
            return View(category);
        }

        private async Task<bool> CategoryExists(int id)
        {
            return await _context.Categories.AnyAsync(e => e.CategoryId == id);
        }

        // GET: CategoryController/Delete/5
        [HttpGet]
        [Route("Delete/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Fetching category with ID {CategoryId} for deletion.", id);
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
            if (category == null)
            {
                _logger.LogWarning("Category with ID {CategoryId} not found for deletion.", id);
                return NotFound();
            }

            return View(category);
        }

        // POST: CategoryController/Delete/5
        [Route("Delete/{id}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _logger.LogInformation("Deleting category with ID {CategoryId}.", id);
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                _logger.LogWarning("Category with ID {CategoryId} not found during deletion.", id);
                return NotFound();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Category with ID {CategoryId} deleted successfully.", id);
            return RedirectToAction(nameof(Index));
        }
    }
}