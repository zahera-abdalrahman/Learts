using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmallBusiness.Data;
using SmallBusiness.Models;

namespace SmallBusiness.Areas.Seller.Controllers
{
    [Area("Seller")]
    public class HomeController : Controller
    {


        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public HomeController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private string GetCurrentUserId()
        {
            return _userManager.GetUserId(User);
        }

        public async Task<IActionResult> Index()
        {

            var sellerId = GetCurrentUserId();

            // Retrieve the profile associated with the current seller
            var profile = await _context.Profile.FirstOrDefaultAsync(p => p.SellerId == sellerId);

            if (profile == null)
            {
                return RedirectToAction("CreateProfile"); // Redirect to create a profile if not exists
            }

            // Retrieve orders with order items that belong to products owned by the current seller
            var orders = await _context.Order
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.OrderItems.Any(oi => oi.Product.ProfileId == profile.ProfileId))
                .ToListAsync();



            // Calculate the total revenue for the current month
            var currentDate = DateTime.Now;
            var startOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            decimal totalRevenueThisMonth = orders
                .Where(o => o.OrderDate >= startOfMonth && o.OrderDate <= endOfMonth)
                .SelectMany(o => o.OrderItems)
                .Sum(oi => oi.Product.ProductPrice);


            // Count the number of customers who bought from the seller
            int numberOfCustomers = orders
                .Select(o => o.UserId)
                .Distinct()
                .Count();


            int totalProductCount = orders
            .SelectMany(o => o.OrderItems)
            .Count();



            var subscription = await _context.Subscription
        .FirstOrDefaultAsync(s => s.SellerId == sellerId && s.SubscriptionEndDate >= currentDate);


            // Get the latest 10 products sold
            var latestProducts = orders
                .SelectMany(o => o.OrderItems)
                .OrderByDescending(oi => oi.Order.OrderDate)
                .Take(10)
                .Select(oi => oi.Product)
                .ToList();

            ViewBag.LatestProducts = latestProducts;


            ViewBag.TotalProductCount = totalProductCount;
            ViewBag.TotalRevenueThisMonth = totalRevenueThisMonth;
            ViewBag.NumberOfCustomers = numberOfCustomers;
            ViewBag.Subscription = subscription;


            return View();

        }
    }
}

