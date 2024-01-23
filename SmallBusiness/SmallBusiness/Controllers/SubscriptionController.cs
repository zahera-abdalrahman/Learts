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
            // Assuming you have some logic to get the current seller's ID

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
                SellerID = viewModel.SellerId,
                SubscriptionStartDate = DateTime.Now,
                SubscriptionEndDate = DateTime.Now.AddMonths(1),
                Price =50,
                status = false
            };

            _context.Subscription.Add(subscription);
            subscription.status = true;

            _context.SaveChanges();

            // Add logic for payment processing here (e.g., integrate with a payment gateway)

            // Redirect to a success page or the default home page
            return RedirectToAction("Index", "Home");
        }
    }
}
