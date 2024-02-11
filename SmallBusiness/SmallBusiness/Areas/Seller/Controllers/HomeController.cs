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

            var profile = await _context.Profile.FirstOrDefaultAsync(p => p.SellerId == sellerId);


            #region subscription

            var currentDate1 = DateTime.Now;

            var subscription1 = await _context.Subscription
                .FirstOrDefaultAsync(s => s.SellerId == sellerId && s.SubscriptionEndDate.Date == currentDate1.Date);

            if (subscription1 != null)
            {
                subscription1.status = false;
                await _context.SaveChangesAsync();
            }

            if (subscription1 != null && !subscription1.status)
            {
                ViewBag.ShowResubscribeButton = true;
            }



            if (Request.Method == "POST")
            {
                if (Request.Form["resubscribe"] == "true")
                {
                    subscription1.status = true;

                    subscription1.SubscriptionStartDate = DateTime.Now;
                    subscription1.SubscriptionEndDate = currentDate1.Date.AddMonths(1);
                    subscription1.Price += 50;

                    await _context.SaveChangesAsync();
                    ViewBag.ShowResubscribeButton = false; 
                }
            }
            #endregion


            //        #region HomePage
            //        var currentDate = DateTime.Now;
            //        var startOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            //        var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            //        var sellerOrderItemsThisMonth = await _context.OrderItems
            //            .Include(oi => oi.Order)
            //            .ThenInclude(o => o.User)
            //            .Include(oi => oi.Product)
            //            .ThenInclude(oi=>oi.Reviews)
            //            .Where(oi => oi.Product.ProfileId == profile.ProfileId &&
            //                         oi.Order.OrderDate >= startOfMonth &&
            //                         oi.Order.OrderDate <= endOfMonth)
            //            .ToListAsync();

            //        decimal totalProductsPriceThisMonth = sellerOrderItemsThisMonth
            //            .Sum(oi => oi.Product.ProductPrice * oi.ProductQuantity);

            //        var orders = await _context.Order
            //            .Include(o => o.User)
            //            .Include(o => o.OrderItems)
            //            .ThenInclude(oi => oi.Product)
            //            .Where(o => o.OrderItems.Any(oi => oi.Product.ProfileId == profile.ProfileId))
            //            .ToListAsync();

            //        int numberOfCustomers = orders
            //            .Select(o => o.UserId)
            //            .Distinct()
            //            .Count();

            //        int totalProductCount = sellerOrderItemsThisMonth
            //.Select(oi => oi.ProductId)
            //.Distinct()
            //.Count();

            //        var subscription = await _context.Subscription
            //            .FirstOrDefaultAsync(s => s.SellerId == sellerId && s.SubscriptionEndDate >= currentDate);

            //        //var latestProducts = orders
            //        //    .SelectMany(o => o.OrderItems)
            //        //    .OrderByDescending(oi => oi.Order.OrderDate)
            //        //    .Take(10)
            //        //    .Select(oi => oi.Product)
            //        //    .ToList();





            //        var subscriptionEndDate = _context.Subscription
            //      .Where(s => s.SellerId == GetCurrentUserId())
            //      .Select(s => s.SubscriptionEndDate)
            //      .FirstOrDefault();

            //        //if (subscriptionEndDate < DateTime.Now)
            //        //{
            //        //    return RedirectToAction("Logout", "Account");
            //        //}

            //        var remainingDays = (subscriptionEndDate - DateTime.Now).Days;

            //        ViewBag.RemainingDays = remainingDays;






            //        ViewBag.sellerOrderItemsThisMonth = sellerOrderItemsThisMonth;
            //        ViewBag.totalProductCount = totalProductCount;
            //        ViewBag.TotalProductsPriceThisMonth = totalProductsPriceThisMonth;
            //        ViewBag.NumberOfCustomers = numberOfCustomers;
            //        ViewBag.Subscription = subscription;
            //        #endregion
            #region HomePage
            var currentDate = DateTime.Now;
            var startOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            var sellerOrderItemsThisMonth = await _context.OrderItems
                .Include(oi => oi.Order)
                .ThenInclude(o => o.User)
                .Include(oi => oi.Product)
                .ThenInclude(oi => oi.Reviews)
                .Where(oi => oi.Product.ProfileId == profile.ProfileId &&
                            oi.Order.OrderDate >= startOfMonth &&
                            oi.Order.OrderDate <= endOfMonth)
                .ToListAsync();

            decimal totalProductsPriceThisMonth = sellerOrderItemsThisMonth
                .Sum(oi => oi.Product.ProductPrice * oi.ProductQuantity);

            // Calculate total quantity of products sold
            int totalProductQuantityThisMonth = sellerOrderItemsThisMonth
                .Sum(oi => oi.ProductQuantity);

            var orders = await _context.Order
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.OrderItems.Any(oi => oi.Product.ProfileId == profile.ProfileId))
                .ToListAsync();

            int numberOfCustomers = orders
                .Select(o => o.UserId)
                .Distinct()
                .Count();

            int totalProductCount = sellerOrderItemsThisMonth
                .Select(oi => oi.ProductId)
                .Distinct()
                .Count();

            var subscription = await _context.Subscription
                .FirstOrDefaultAsync(s => s.SellerId == sellerId && s.SubscriptionEndDate >= currentDate);

            var subscriptionEndDate = _context.Subscription
    .Where(s => s.SellerId == GetCurrentUserId())
    .Select(s => s.SubscriptionEndDate)
    .FirstOrDefault();

            ViewBag.SubscriptionEndDate = subscriptionEndDate;

            var remainingDays = (subscriptionEndDate - DateTime.Now).Days;

            ViewBag.RemainingDays = remainingDays;
            ViewBag.sellerOrderItemsThisMonth = sellerOrderItemsThisMonth;
            ViewBag.totalProductCount = totalProductCount;
            ViewBag.TotalProductsPriceThisMonth = totalProductsPriceThisMonth;
            ViewBag.NumberOfCustomers = numberOfCustomers;
            ViewBag.Subscription = subscription;
            ViewBag.TotalProductQuantityThisMonth = totalProductQuantityThisMonth; // Add this line to pass total product quantity to the view
            #endregion

            return View();
        }



    }
}

