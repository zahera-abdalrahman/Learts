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



        public async Task<IActionResult> ProductSaleAsync(string categoryName, string searchProductName, string sortOrder)
        {

            #region cart
            User user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                Cart cart = _context.Cart.FirstOrDefault(c => c.UserId == user.Id);

                if (cart != null)
                {
                    var cartItemsModified = _context
                        .CartItems
                        .Include(ci => ci.Product)
                        .Where(ci => ci.CartId == cart.CartId)
                        .ToList();

                    ViewBag.CartItems = cartItemsModified;
                }
            }
            #endregion

            #region Fav
            var favoriteProducts = _context.Favorite.Include(c => c.Product).Where(p => p.IsFav).ToList();

            ViewBag.Fav = favoriteProducts;
            #endregion

            List<Product> categoryProducts;

            if (string.IsNullOrEmpty(categoryName))
            {
                categoryName = "All";
            }

            // Filter by category
            if (categoryName.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                categoryProducts = _context.Product.Include(p => p.Category).Where(p => p.ProductSale > 0).ToList();
            }
            else
            {
                categoryProducts = _context
                    .Product
                    .Include(p => p.Category)
                    .Where(p => p.Category != null && p.Category.CategoryName == categoryName && p.ProductSale > 0)
                    .ToList();
            }

            // Filter by product name if provided
            if (!string.IsNullOrEmpty(searchProductName))
            {
                categoryProducts = categoryProducts
                    .Where(p => p.ProductName.Contains(searchProductName, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            switch (sortOrder)
            {
                case "rating":
                    categoryProducts = categoryProducts.OrderByDescending(p => p.ReviewRate).ToList();
                    break;

                case "date":
                    categoryProducts = categoryProducts.OrderByDescending(p => p.CreateAt).ToList();
                    break;

                case "price":
                    categoryProducts = categoryProducts.OrderBy(p => p.ProductPrice).ToList();
                    break;

                case "price-desc":
                    categoryProducts = categoryProducts.OrderByDescending(p => p.ProductPrice).ToList();
                    break;

                // Add more cases for additional sorting options if needed

                default:
                    break;
            }

            var discountedProducts = categoryProducts
                .Select(
                    p =>
                        new
                        {
                            ProductId = p.ProductId,
                            ProductName = p.ProductName,
                            Description = p.ProductDescription,
                            Price = p.ProductPrice,
                            QuantityInStock = p.ProductQuantityStock,
                            CategoryId = p.CategoryId,
                            Category = p.Category,
                            DiscountPercent = p.ProductSale,
                            Rate=p.ReviewRate,
                            ImageUrl = p.ImageUrl,
                            DiscountedPrice = p.ProductPrice - (p.ProductPrice * p.ProductSale / 100)
                        }
                )
                .ToList();

            if (discountedProducts.Any())
            {
                ViewBag.productList = discountedProducts;
            }
            else
            {
                ViewBag.productList = null;
                ViewBag.NoProductsMessage = !string.IsNullOrEmpty(searchProductName)
                    ? $"No products found for \"{searchProductName}\"."
                    : "No products found.";
            }

            var categoryList = _context.Category.Where(c => c.IsDelete == false).ToList();
            var categoryProductCount = new Dictionary<int, int>();
            foreach (var category in categoryList)
            {
                int productCount = _context.Product
                    .Count(p => p.Category != null && p.Category.CategoryId == category.CategoryId && p.ProductSale > 0);
                categoryProductCount.Add(category.CategoryId, productCount);
            }

            ViewBag.SearchTerm = searchProductName;

            ViewBag.categoryList = categoryList;
            ViewBag.categoryCount = discountedProducts.Count;
            ViewBag.ALL = categoryProducts.Count;

            ViewBag.categoryProductCount = categoryProductCount;

            return View();
        }



        public async Task<IActionResult> productsAsync(string categoryName, string searchProductName, string sortOrder)
        {


            #region cart
            User user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                Cart cart = _context.Cart.FirstOrDefault(c => c.UserId == user.Id);

                if (cart != null)
                {
                    var cartItemsModified = _context
                        .CartItems
                        .Include(ci => ci.Product)
                        .Where(ci => ci.CartId == cart.CartId)
                        .ToList();

                    ViewBag.CartItems = cartItemsModified;
                }
            }
            #endregion
            #region Fav
            var favoriteProducts = _context.Favorite.Include(c => c.Product).Where(p => p.IsFav).ToList();

            ViewBag.Fav = favoriteProducts;
            #endregion

            List<Product> categoryProducts;

            if (string.IsNullOrEmpty(categoryName))
            {
                categoryName = "All";
            }

            if (categoryName.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                categoryProducts = _context.Product.Include(p => p.Category).Include(p => p.Reviews).ToList();
            }
            else
            {
                categoryProducts = _context
                    .Product
                    .Include(p => p.Category)
                    .Include(p=>p.Reviews)
                    .Where(p => p.Category != null && p.Category.CategoryName == categoryName)
                    .ToList();
            }

            if (!string.IsNullOrEmpty(searchProductName))
            {
                categoryProducts = categoryProducts
                    .Where(p => p.ProductName.Contains(searchProductName, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            switch (sortOrder)
            {
                case "rating":
                    categoryProducts = categoryProducts
                        .OrderByDescending(p => p.Reviews.Any() ? p.Reviews.Average(r => r.ReviewRate) : 0)
                        .ToList();
                    break;

                case "date":
                    categoryProducts = categoryProducts.OrderByDescending(p => p.CreateAt).ToList();
                    break;

                case "price":
                    categoryProducts = categoryProducts.OrderBy(p => p.ProductPrice).ToList();
                    break;

                case "price-desc":
                    categoryProducts = categoryProducts.OrderByDescending(p => p.ProductPrice).ToList();
                    break;

                default:
                    break;
            }

            var discountedProducts = categoryProducts
                .Select(
                    p =>
                        new
                        {
                            ProductId = p.ProductId,
                            ProductName = p.ProductName,
                            Description = p.ProductDescription,
                            Price = p.ProductPrice,
                            QuantityInStock = p.ProductQuantityStock,
                            CategoryId = p.CategoryId,
                            Category = p.Category,
                            DiscountPercent = p.ProductSale,
                            ReviewRate = p.Reviews.Any() ? p.Reviews.Average(r => r.ReviewRate) : 0,
                            ImageUrl = p.ImageUrl,
                            OldPrice = p.ProductPrice,
                            NewPrice = p.ProductPrice - (p.ProductPrice * p.ProductSale / 100)
                        }
                )
                .ToList();

            if (discountedProducts.Any())
            {
                ViewBag.productList = discountedProducts;
            }
            else
            {
                ViewBag.productList = null;
                ViewBag.NoProductsMessage = !string.IsNullOrEmpty(searchProductName)
                    ? $"No products found for \"{searchProductName}\"."
                    : "No products found.";
            }

            var categoryList = _context.Category.Where(c => c.IsDelete == false).ToList();
            var categoryProductCount = new Dictionary<int, int>();
            foreach (var category in categoryList)
            {
                int productCount = _context.Product
                    .Count(p => p.Category != null && p.Category.CategoryId == category.CategoryId);
                categoryProductCount.Add(category.CategoryId, productCount);
            }

            ViewBag.SearchTerm = searchProductName;

            ViewBag.categoryList = categoryList;
            ViewBag.categoryCount = discountedProducts.Count;
            ViewBag.ALL = categoryProducts.Count;

            ViewBag.categoryProductCount = categoryProductCount;

            return View();
        }






        public async Task<IActionResult> productDetails(int productId)
        {
            var product = await _context.Product
                .Include(p => p.Category)
                .Include(p => p.profile) 
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            var reviews = await _context.Review
        .Include(r => r.User)
        .Include(r => r.Product)
        .Where(r => r.ProductId == productId && r.isActive) 
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
                DiscountedPrice = discountedPrice,
                ProfileId = product.ProfileId, 
                Profile = product.profile,
                DiscountPercent = product.ProductSale,
                Rate = product.ReviewRate,
                OldPrice = product.ProductPrice,
                NewPrice = product.ProductPrice - (product.ProductPrice * product.ProductSale / 100)
            };

            ViewBag.Reviews = reviews;

            #region cart
            User user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                Cart cart = _context.Cart.FirstOrDefault(c => c.UserId == user.Id);

                if (cart != null)
                {
                    var cartItemsModified = _context
                        .CartItems
                        .Include(ci => ci.Product)
                        .Where(ci => ci.CartId == cart.CartId)
                        .ToList();

                    ViewBag.CartItems = cartItemsModified;
                }
            }
            #endregion


            #region Fav
            var favoriteProducts = _context.Favorite.Include(c => c.Product).Where(p => p.IsFav).ToList();

            ViewBag.Fav = favoriteProducts;
            #endregion
            return View();
        }

        private decimal CalculateDiscountedPrice(decimal originalPrice, decimal discountPercent)
        {
            return originalPrice - (originalPrice * discountPercent / 100);
        }



        #region review
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Review(Review model, int ProductId)
        {
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
                ProductId = ProductId, 
                ReviewDate = DateTime.Now,
                ReviewRate = model.ReviewRate,
                ReviewMessage = model.ReviewMessage,
                isActive = false,
                User = user
            };

            _context.Review.Add(newReview);

            try
            {
                await _context.SaveChangesAsync();
                var swalScript = "Swal.fire('Review Submitted', 'Your review has been submitted and will be reviewed by the admin.', 'success');";
                TempData["SweetAlertScript"] = swalScript;
            }
            catch (DbUpdateException ex)
            {

                return BadRequest($"Error adding review: {ex.Message}");
            }

            return RedirectToAction("productDetails", new { productId = ProductId });
        }
        #endregion


    }

}
