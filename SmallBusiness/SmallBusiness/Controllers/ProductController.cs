using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmallBusiness.Data;
using SmallBusiness.Models;

namespace SmallBusiness.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _context;

        public ProductController(UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<User> signInManager,
            ILogger<ProductController> logger,
            ApplicationDbContext context
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
        }

        public IActionResult products()
        {
            var products = _context.Product
                .Include(p => p.Category) 
                .Include(p => p.profile)  
                .ToList();

            return View(products);
        }

        public async Task<IActionResult> productDetails(int productId)
        {
            var product = _context.Product
                .Include(p => p.Category)
                .FirstOrDefault(p => p.ProductId == productId);

            var reviews = await _context.Review
                .Include(r => r.User)
                .Include(r=>r.Product)
                .Where(r => r.isActive)
                .Take(8)
                .ToListAsync();
            if (product == null)
            {
                return NotFound();
            }

            var discountedPrice = CalculateDiscountedPrice(
                (decimal)product.ProductPrice,
                product.ProductSale
            );

            ViewBag.product = new
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductDescription = product.ProductDescription,
                ProductPrice = product.ProductPrice,
                ProductQuantityStock = product.ProductQuantityStock,
                CategoryId = product.CategoryId,
                Category = product.Category,
                ProductSale = product.ProductSale,
                ImageUrl = product.ImageUrl,
                DiscountedPrice = discountedPrice
            };

            ViewBag.Reviews = reviews;



            return View();
        }

        private decimal CalculateDiscountedPrice(decimal originalPrice, decimal discountPercent)
        {
            return originalPrice - (originalPrice * discountPercent / 100);
        }




        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Review(Review model, int ProductId)
        {
            // Check if the product with the specified ProductId exists
            var productExists = await _context.Product.AnyAsync(p => p.ProductId == ProductId);
            if (!productExists)
            {
                return NotFound("Product not found");
            }

            var user = await _userManager.GetUserAsync(User);

            var newReview = new Review
            {
                UserId = user.Id,
                Name = model.Name,
                Email = model.Email,
                ProductId = ProductId, // Set the correct ProductId here
                ReviewDate = DateTime.Now,
                ReviewRate=model.ReviewRate,
                ReviewMessage = model.ReviewMessage,
                isActive = false,
                User = user
            };

            _context.Review.Add(newReview);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Handle specific database update exceptions here
                // You can log the exception or provide a meaningful error message
                return BadRequest($"Error adding review: {ex.Message}");
            }

            return RedirectToAction("productDetails", new { productId = ProductId });
        }

    }

}
