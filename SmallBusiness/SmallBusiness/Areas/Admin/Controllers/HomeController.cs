using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmallBusiness.Data;
using SmallBusiness.Models;

namespace SmallBusiness.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {


        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public HomeController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task<IActionResult> IndexAsync()
        {
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var ordersForCurrentMonth = await _context.Order
                .Include(order => order.User)
                .Where(order => order.OrderDate.Month == currentMonth && order.OrderDate.Year == currentYear)
                .OrderByDescending(order => order.OrderDate)
                .Take(10)
                .ToListAsync();

            ViewBag.OrderCount = ordersForCurrentMonth.Count;

            decimal totalForCurrentMonth = ordersForCurrentMonth.Sum(order => order.TotalPrice);
            ViewBag.TotalForCurrentMonth = totalForCurrentMonth;

            var products = await _context.Product.ToListAsync();
            ViewBag.ProductCount = products.Count;

            var category = await _context.Category.ToListAsync();
            ViewBag.CategoryCount = category.Count;



            var userCount = await _userManager.GetUsersInRoleAsync("User");
            ViewBag.UserCount = userCount.Count;
           

            var sellerCount = await _userManager.GetUsersInRoleAsync("Seller");
            ViewBag.SellerCount = sellerCount.Count;

            return View(ordersForCurrentMonth);
        }

    }
}
