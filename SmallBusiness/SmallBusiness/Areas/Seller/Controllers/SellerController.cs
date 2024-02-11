using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using NuGet.Protocol.Plugins;
using SmallBusiness.Data;
using SmallBusiness.Models;
using SmallBusiness.ViewModels;

namespace SmallBusiness.Controllers
{
    [Area("Seller")]
    public class SellerController : Controller
    {


        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public SellerController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

      

        [Authorize(Roles = "Seller")] 
        public async Task<IActionResult> MyProfileAsync()
        {

            var isSubscriptionActive = await CheckSubscriptionStatus();

            if (!isSubscriptionActive)
            {
                TempData["Message"] = "Please renew your subscription to access product listing.";
                return RedirectToAction("Index", "Home"); // Redirect to a page for renewing subscription
            }
            var userId = GetCurrentUserId();

            var profile = _context.Profile.FirstOrDefault(p => p.SellerId == userId);

            if (profile == null)
            {
                return RedirectToAction("Profile");
            }

            var viewModel = new ProfileViewModel
            {
                ProfileId = profile.ProfileId,
                ProfileImage = profile.ProfileImage,
                ShopName = profile.ShopName,
                Description = profile.Description,
                IsApproved = profile.IsApproved,
                FacebookLink = profile.FacebookLink,
                InstagramLink = profile.InstagramLink,
                PinterestLink = profile.PinterestLink,
                Seller = profile.Seller,
                SellerId = profile.SellerId,
            };

            return View(viewModel);
        }


        /////////////////////////////////////

        #region Profile
        public IActionResult Profile(string userId)
        {
           
            var viewModel = new ProfileViewModel
            {
                UserId = userId
            };

            return View(viewModel);

        }

       
        [HttpPost]
        public async Task<IActionResult> Profile(ProfileViewModel viewModel, [FromServices] IWebHostEnvironment host)
        {
            string ImageName = "";
            if (viewModel.File != null)
            {
                string PathImage = Path.Combine(host.WebRootPath, "Profile");
                FileInfo fi = new FileInfo(viewModel.File.FileName);
                ImageName = "Image" + DateTime.UtcNow.ToString().Replace("/", "").Replace(":", "").Replace("-", "").Replace(" ", "") + fi.Extension;

                string FullPath = Path.Combine(PathImage, ImageName);
                viewModel.File.CopyTo(new FileStream(FullPath, FileMode.Create));
            }


            var user = await _userManager.FindByIdAsync(viewModel.UserId);

            var profile = new Profile
            {
                ProfileImage = ImageName,
                ShopName = viewModel.ShopName,
                Description = viewModel.Description,
                IsApproved = false, // Modify as needed
                SellerId = user.Id,
                FacebookLink = viewModel.FacebookLink,
                InstagramLink = viewModel.InstagramLink,
                PinterestLink = viewModel.PinterestLink,
            };

            _context.Profile.Add(profile);
            _context.SaveChanges();

            return RedirectToAction("CreateSubscription", "Subscription", new { area = "", userId = user.Id });

        }

        public async Task<IActionResult> EditProfileAsync()
        {
            var userId = GetCurrentUserId();

            var profile = _context.Profile.FirstOrDefault(p => p.SellerId == userId);

            if (profile == null)
            {
                return RedirectToAction("Profile");
            }

            var viewModel = new ProfileViewModel
            {
                ProfileId = profile.ProfileId,
                ProfileImage = profile.ProfileImage,
                ShopName = profile.ShopName,
                Description = profile.Description,
                IsApproved = profile.IsApproved,
                FacebookLink = profile.FacebookLink,
                InstagramLink = profile.InstagramLink,
                PinterestLink = profile.PinterestLink,
                Seller=profile.Seller,
                SellerId=profile.SellerId,
                
            };

            return View(viewModel);
        }



        [HttpPost]
        public IActionResult EditProfile(int profileId, ProfileViewModel viewModel, [FromServices] IWebHostEnvironment host)
        {

            string ImageName = "";
            if (viewModel.File != null)
            {
                string PathImage = Path.Combine(host.WebRootPath, "Profile");
                FileInfo fi = new FileInfo(viewModel.File.FileName);
                ImageName = "Image" + DateTime.UtcNow.ToString().Replace("/", "").Replace(":", "").Replace("-", "").Replace(" ", "") + fi.Extension;

                string FullPath = Path.Combine(PathImage, ImageName);
                viewModel.File.CopyTo(new FileStream(FullPath, FileMode.Create));
            }
            else
            {
                ImageName = viewModel.ProfileImage;
            }

            var sellerId = GetCurrentUserId();
            var profile = _context.Profile.FirstOrDefault(p => p.SellerId == sellerId);

            if (profile == null)
            {
                return RedirectToAction("Profile");
            }

             
                profile.ProfileImage = ImageName;
                profile.ShopName = viewModel.ShopName;
                profile.Description = viewModel.Description;
                profile.IsApproved = viewModel.IsApproved;
                profile.FacebookLink= viewModel.FacebookLink;
                profile.InstagramLink = viewModel.InstagramLink;
                profile.PinterestLink= viewModel.PinterestLink;

                _context.Profile.Update(profile);
                _context.SaveChanges();

            
            return View("EditProfile",viewModel);
        }
        private string GetCurrentUserId()
        {
            return _userManager.GetUserId(User);
        }

        #endregion
        private async Task<bool> CheckSubscriptionStatus()
        {
            var sellerId = GetCurrentUserId();

            var subscription = await _context.Subscription.FirstOrDefaultAsync(s => s.SellerId == sellerId);

            return subscription != null && subscription.status;
        }
    }
}
