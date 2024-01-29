using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmallBusiness.Data;
using SmallBusiness.Models;
using SmallBusiness.ViewModels;

namespace SmallBusiness.Areas.Seller.Controllers
{
    [Area("Seller")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public ProductController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;

        }


        // GET: Seller/Product
        public async Task<IActionResult> Index()
        {
            var sellerId = GetCurrentUserId();

            // Retrieve the profile associated with the current seller
            var profile = await _context.Profile.FirstOrDefaultAsync(p => p.SellerId == sellerId);

            if (profile == null)
            {
                return RedirectToAction("CreateProfile"); // Redirect to create a profile if not exists
            }

            // Retrieve products that belong to the current seller's profile
            var products = await _context.Product
                .Where(p => !p.IsDelete && p.ProfileId == profile.ProfileId)
                .Include(p => p.Category)
                .Include(p => p.profile)
                .ToListAsync();

            return View(products);

        }


        // GET: Seller/Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.Category)
                .Include(p => p.profile)
                .FirstOrDefaultAsync(m => m.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Seller/Product/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "CategoryName");
            ViewData["ProfileId"] = new SelectList(_context.Profile, "ProfileId", "Description");
            return View();
        }
        private string GetCurrentUserId()
        {
            return _userManager.GetUserId(User);
        }
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel viewModel, [FromServices] IWebHostEnvironment host)
        {
            string ImageName = "";
            if (viewModel.File != null)
            {
                string PathImage = Path.Combine(host.WebRootPath, "CategoryImg");
                FileInfo fi = new FileInfo(viewModel.File.FileName);
                ImageName = "Image" + DateTime.UtcNow.ToString().Replace("/", "").Replace(":", "").Replace("-", "").Replace(" ", "") + fi.Extension;

                string FullPath = Path.Combine(PathImage, ImageName);
                viewModel.File.CopyTo(new FileStream(FullPath, FileMode.Create));
            }


            var sellerId = GetCurrentUserId();
            var profile = _context.Profile.FirstOrDefault(p => p.SellerId == sellerId);

            if (profile == null)
            {
                return RedirectToAction("Index");
            }


            var product = new Product
            {
                ProductName = viewModel.ProductName,
                ProductDescription = viewModel.ProductDescription,
                ProductPrice = viewModel.ProductPrice,
                ProductQuantityStock = viewModel.ProductQuantityStock,
                ProductSale = viewModel.ProductSale,
                ImageUrl = ImageName,
                CategoryId = viewModel.CategoryId,
                ProfileId = profile.ProfileId,
                CreateAt = DateTime.UtcNow,
                ReviewRate = viewModel.ReviewRate,
            };

            _context.Product.Add(product);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }










        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            // Ensure that the product belongs to the current seller
            var sellerId = GetCurrentUserId();
            var profile = await _context.Profile.FirstOrDefaultAsync(p => p.SellerId == sellerId);

            if (profile == null || product.ProfileId != profile.ProfileId)
            {
                return NotFound(); // Redirect or show an error if the product doesn't belong to the current seller
            }

            // Convert the Product entity to ProductViewModel
            var productViewModel = new ProductViewModel
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductDescription = product.ProductDescription,
                ProductPrice = product.ProductPrice,
                ProductQuantityStock = product.ProductQuantityStock,
                ProductSale = product.ProductSale,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
            };

            ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "CategoryName", product.CategoryId);
            ViewData["ProfileId"] = new SelectList(_context.Profile, "ProfileId", "Description", product.ProfileId);

            return View(productViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Edit(int id, ProductViewModel viewModel, [FromServices] IWebHostEnvironment host)
        {
            string ImageName = "";
            if (viewModel.File != null)
            {
                string PathImage = Path.Combine(host.WebRootPath, "CategoryImg");
                FileInfo fi = new FileInfo(viewModel.File.FileName);
                ImageName = "Image" + DateTime.UtcNow.ToString().Replace("/", "").Replace(":", "").Replace("-", "").Replace(" ", "") + fi.Extension;

                string FullPath = Path.Combine(PathImage, ImageName);
                viewModel.File.CopyTo(new FileStream(FullPath, FileMode.Create));
            }
            else
            {
                ImageName = viewModel.ImageUrl;
            }

            var sellerId = GetCurrentUserId();
            var profile = _context.Profile.FirstOrDefault(p => p.SellerId == sellerId);

            if (profile == null)
            {
                return RedirectToAction("Index");
            }

            // Retrieve the existing product from the database
            var existingProduct = await _context.Product.FirstOrDefaultAsync(p => p.ProductId == id && p.ProfileId == profile.ProfileId);

            if (existingProduct == null)
            {
                return NotFound(); // Product not found or doesn't belong to the current seller
            }

            // Update the properties of the existing product
            existingProduct.ProductName = viewModel.ProductName;
            existingProduct.ProductDescription = viewModel.ProductDescription;
            existingProduct.ProductPrice = viewModel.ProductPrice;
            existingProduct.ProductQuantityStock = viewModel.ProductQuantityStock;
            existingProduct.ProductSale = viewModel.ProductSale;
            existingProduct.ImageUrl = ImageName;
            existingProduct.CategoryId = viewModel.CategoryId;
            existingProduct.ReviewRate = viewModel.ReviewRate;
            existingProduct.ProductId = viewModel.ProductId;

            _context.Update(existingProduct);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }








        // GET: Seller/Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.Category)
                .Include(p => p.profile)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Seller/Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                if (_context.Product == null)
                {
                    return Problem("Entity set 'ApplicationDbContext.Product' is null.");
                }

                var product = await _context.Product.FindAsync(id);

                if (product == null)
                {
                    return NotFound();
                }

                // Soft delete by setting the IsDeleted flag
                product.IsDelete = true;
                _context.Entry(product).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Handle exceptions, log, or return an error view
                return RedirectToAction("Index"); // You may want to redirect to an error page
            }
        }

        private bool ProductExists(int id)
        {
          return (_context.Product?.Any(e => e.ProductId == id)).GetValueOrDefault();
        }
    }
}
