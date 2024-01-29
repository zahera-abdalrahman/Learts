using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmallBusiness.Data;
using SmallBusiness.Models;
using SmallBusiness.ViewModels;
using System.Security.Claims;

namespace SmallBusiness.Controllers
{
    public class SubscriptionController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public SubscriptionController(
            ILogger<HomeController> logger,
            ApplicationDbContext context,
            UserManager<User> userManager
            )
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public IActionResult CreateSubscription(string userId)
        {

            var viewModel = new SubscriptionViewModel
            {
                SellerId = userId
               
            };

            return View(viewModel);
        }

       
        [HttpPost]
        public IActionResult CreateSubscription(SubscriptionViewModel viewModel)
        {
           
        
            var subscription = new Subscription
            {
                SellerId = viewModel.SellerId,
                SubscriptionStartDate = DateTime.Now,
                SubscriptionEndDate = DateTime.Now.AddMonths(1),
                Price =50,
                status = false,
            };

            _context.Subscription.Add(subscription);
            subscription.status = true;

            _context.SaveChanges();

            TempData["SellerConfirmation"] = true;

            return RedirectToAction("Index", "Home");
        }
    }
}
