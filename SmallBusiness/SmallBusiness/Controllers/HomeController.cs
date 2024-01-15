using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmallBusiness.Data;
using SmallBusiness.Models;
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
        public async Task<IActionResult> IndexAsync()
        {
            DateTime lastWeek = DateTime.Now.AddDays(-7);
            var newProducts = _context.Product.Where(p => p.CreateAt >= lastWeek).ToList();

            var mostRatedProducts = _context.Product
                   .Where(p => p.ReviewRate == 5)
                   .OrderByDescending(p => p.ReviewRate)
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
                .ToList();


            var reviews = await _context.Testimonial
                .Include(r => r.User)
                .Where(r => r.isActive)
                .Take(8)
                .ToListAsync();

            ViewBag.Reviews = reviews;


            ViewBag.categoryList = _context.Category.ToList();
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

                return RedirectToAction("Index");
           
        }

        #endregion


        #region About
        public IActionResult About()
        {
            return View();
        }

        #endregion




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