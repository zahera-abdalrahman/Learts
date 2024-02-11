using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmallBusiness.Data;
using SmallBusiness.Models;
using SmallBusiness.ViewModels;
using System.Diagnostics;

namespace SmallBusiness.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _context;

        public HomeController(UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<User> signInManager,
            ILogger<HomeController> logger,
            ApplicationDbContext context
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
        }


        #region Home
        public async Task<IActionResult> IndexAsync(int productId)
        {
            var newProducts = _context.Product
        .OrderByDescending(p => p.CreateAt)
        .Take(10)
        .ToList();

            var mostRatedProducts = _context.Product
         .Include(p => p.Reviews) 
         .Where(p => p.Reviews.Any(r => r.ReviewRate == 5))
         .OrderByDescending(p => p.Reviews.Count(r => r.ReviewRate == 5))
         .Take(10)
         .ToList();
            var productsWithSales = _context.Product.Where(p => p.ProductSale > 0).ToList();


            var discountedProducts = productsWithSales
                .Select(p => new
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Description = p.ProductDescription,
                    Price = p.ProductPrice,
                    QuantityInStock = p.ProductQuantityStock,
                    CategoryId = p.CategoryId,
                    Category = p.Category,
                    DiscountPercent = p.ProductSale,
                    ImageUrl = p.ImageUrl,
                    OldPrice = p.ProductPrice,
                    NewPrice = p.ProductPrice - (p.ProductPrice * p.ProductSale / 100)
                })
                .Take(10)
                .ToList();


            var reviews = await _context.Testimonial
                .Include(r => r.User)
                .Where(r => r.isActive)
                .Take(8)
                .ToListAsync();

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


            ViewBag.Reviews = reviews;


            ViewBag.categoryList = _context.Category.Where(c => c.IsDelete == false).ToList();


            ViewBag.NewProducts = newProducts;
            ViewBag.MostRatedProducts = mostRatedProducts;
            ViewBag.DiscountedProducts = discountedProducts;

            return View();
        }



        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Testimonial(Testimonial model)
        {


            var user = await _userManager.GetUserAsync(User);
            var newReview = new Testimonial
            {
                UserId = user.Id,
                Name = model.Name,
                Email = model.Email,
                TestimonialMessage = model.TestimonialMessage,
                isActive = false,
                User = user
            };
            _context.Testimonial.Add(newReview);
            await _context.SaveChangesAsync();
            var swalScript = "Swal.fire('Review Submitted', 'Your review has been submitted and will be reviewed by the admin.', 'success');";
            TempData["SweetAlertScript"] = swalScript;
            return RedirectToAction("Index");

        }

        #endregion


        #region About
        public async Task<IActionResult> AboutAsync()
        {

            //#region Fav
            //var favoriteProducts = _context.Product.Where(p => p.IsFav).ToList();

            //ViewBag.Fav = favoriteProducts;
            //#endregion


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

            return View();

        }

        #endregion




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
            return PartialView("_ProductDetailsModal");
        }


        private decimal CalculateDiscountedPrice(decimal originalPrice, decimal discountPercent)
        {
            return originalPrice - (originalPrice * discountPercent / 100);
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}