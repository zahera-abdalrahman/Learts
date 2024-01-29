using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmallBusiness.Data;
using SmallBusiness.Models;
using SmallBusiness.ViewModels;

namespace SmallBusiness.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Categories
        public async Task<IActionResult> Index()
        {
              return _context.Category != null ? 
                          View(await _context.Category.Where(c=>c.IsDelete==false).ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Category'  is null.");
        }

        // GET: Admin/Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Category == null)
            {
                return NotFound();
            }

            var category = await _context.Category
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Admin/Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Administrator/ProductCategory/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel productCategory, [FromServices] IWebHostEnvironment host)
        {
           
            string ImageName = "";
            if (productCategory.File != null)
            {
                string PathImage = Path.Combine(host.WebRootPath, "CategoryImg");
                FileInfo fi = new FileInfo(productCategory.File.FileName);
                ImageName = "Image" + DateTime.UtcNow.ToString().Replace("/", "").Replace(":", "").Replace("-", "").Replace(" ", "") + fi.Extension;

                string FullPath = Path.Combine(PathImage, ImageName);
                productCategory.File.CopyTo(new FileStream(FullPath, FileMode.Create));
            }
            
            var newCat = new Category
            {
                CategoryId = productCategory.CategoryId,
                CategoryName = productCategory.CategoryName,
                Image = ImageName
            };
            _context.Add(newCat);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
        // GET: Admin/Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Category == null)
            {
                return NotFound();
            }

            var productCategory = await _context.Category.FindAsync(id);
            var newCat = new CategoryViewModel
            {
                CategoryId = productCategory.CategoryId,
                CategoryName = productCategory.CategoryName,
                Image = productCategory.Image
            };
            if (productCategory == null)
            {
                return NotFound();
            }
            return View(newCat);
        }

        // POST: Administrator/ProductCategory/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryViewModel productCategory, [FromServices] IWebHostEnvironment host)
        {
            if (id != productCategory.CategoryId)
            {
                return NotFound();
            }
            string ImageName = "";
            if (productCategory.File != null)
            {
                string PathImage = Path.Combine(host.WebRootPath, "CategoryImg");
                FileInfo fi = new FileInfo(productCategory.File.FileName);
                ImageName = "Image" + DateTime.UtcNow.ToString().Replace("/", "").Replace(":", "").Replace("-", "").Replace(" ", "") + fi.Extension;

                string FullPath = Path.Combine(PathImage, ImageName);
                productCategory.File.CopyTo(new FileStream(FullPath, FileMode.Create));
            }
            else
            {
                ImageName = productCategory.Image;
            }
            var newCat = new Category
            {
                CategoryId = productCategory.CategoryId,
                CategoryName = productCategory.CategoryName,
                Image = ImageName
            };
            _context.Update(newCat);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        // GET: Admin/Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Category == null)
            {
                return NotFound();
            }

            var category = await _context.Category
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Admin/Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                if (_context.Category == null)
                {
                    return Problem("Entity set 'ApplicationDbContext.Category' is null.");
                }

                var category = await _context.Category.FindAsync(id);

                if (category == null)
                {
                    return NotFound();
                }

                // Soft delete by setting the IsDeleted flag
                category.IsDelete = true;
                _context.Entry(category).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Categories");
            }
            catch (Exception ex)
            {
                // Log the exception or handle it according to your application's requirements
                return Problem($"An error occurred while deleting the category. {ex.Message}");
            }
        }


        private bool CategoryExists(int id)
        {
          return (_context.Category?.Any(e => e.CategoryId == id)).GetValueOrDefault();
        }
    }
}
