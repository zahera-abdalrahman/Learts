using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SmallBusiness.Data;
using SmallBusiness.Models;
using SmallBusiness.ViewModels;

namespace SmallBusiness.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public ProfileController(
            ILogger<HomeController> logger,
            ApplicationDbContext context,
            UserManager<User> userManager
            )
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }


        public IActionResult AllSellers()
        {
            var allSellers = _context.Profile
                .Where(p => p.SellerId != null)
                .Include(p => p.Seller)
                .ToList();

            ViewBag.AllSellers = allSellers;

            return View();
        }

      
        public IActionResult ProfileDetails(int profileId)
            {
                var profile = _context.Profile
                    .Include(p => p.Seller) 
                    .FirstOrDefault(p => p.ProfileId == profileId);

                if (profile == null)
                {
                    return NotFound();
                }

                var products = _context.Product
                    .Include(p => p.Category)
                    .Where(p => p.ProfileId == profileId)
                    .ToList();

                var viewModel = new ProfileDetailsViewModel
                {
                    Profile = profile,
                    Products = products
                };

                 ViewBag.ProfileDetails = viewModel;

                return View();
            }
    }
}
