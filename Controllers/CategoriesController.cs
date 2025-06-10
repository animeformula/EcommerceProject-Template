using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using EcommerceProject.Data;
using EcommerceProject.Models;
using EcommerceProject.ViewModels;

// This is the CategoriesController class that handles the categories in the application

namespace EcommerceProject.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public CategoriesController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // This is the Index action that displays the categories
        public async Task<IActionResult> Index()
        {
            // This is the query to get the categories
            var categories = await _context.Categories
                .Include(c => c.Products)
                .ToListAsync();
            return View(categories);
        }

        // This is the Details action that displays the category details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View(new CategoryCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CategoryCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Handle file upload
            string imageUrl = null;
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "categories");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(fileStream);
                }
                imageUrl = "/images/categories/" + uniqueFileName;
            }
            else
            {
                ModelState.AddModelError("ImageFile", "Please upload a category image.");
                return View(model);
            }

            var category = new Category
            {
                Name = model.Name,
                Description = model.Description,
                ImageUrl = imageUrl,
                CreatedAt = DateTime.UtcNow
            };
            _context.Add(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,ImageUrl")] Category category, IFormFile imageFile)
        {
            if (id != category.Id)
                return NotFound();

            // Remove validation for navigation/collection properties
            ModelState.Remove("Products");

            if (!ModelState.IsValid)
                return View(category);

            var originalCategory = await _context.Categories.FindAsync(id);
            if (originalCategory == null)
                return NotFound();

            // Update fields
            originalCategory.Name = category.Name;
            originalCategory.Description = category.Description;

            // Handle image
            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "categories");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid() + "_" + imageFile.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                // Optionally delete old image if local
                if (!string.IsNullOrEmpty(originalCategory.ImageUrl) && originalCategory.ImageUrl.StartsWith("/images/categories/"))
                {
                    var oldImagePath = Path.Combine(_environment.WebRootPath, originalCategory.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                        System.IO.File.Delete(oldImagePath);
                }

                originalCategory.ImageUrl = "/images/categories/" + uniqueFileName;
            }
            else
            {
                // Use the value from the text field (can be remote URL or left unchanged)
                originalCategory.ImageUrl = category.ImageUrl;
            }

            originalCategory.UpdatedAt = DateTime.UtcNow;

            _context.Update(originalCategory);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
} 