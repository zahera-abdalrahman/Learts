using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SmallBusiness.Data;
using SmallBusiness.Models;
using SmallBusiness.ViewModels;

namespace SmallBusiness.Controllers
{
    public class UserDetailsController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _context;

        public UserDetailsController(UserManager<User> userManager,
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

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> UserDetails()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return View();
            }

            var userOrders = _context.Order.Where(o => o.UserId == user.Id).ToList();

            var UserDetails = new UserDetailsViewModel
            {
                User = user,
                Orders = userOrders,
                EditModel = new UpdateViewModel
                {
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    City = user.City,
                    CurrentPassword = "", 
                    NewPassword = "",
                    ConfirmPassword = ""
                }
            };
            #region cart

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
            return View(UserDetails);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(UserDetailsViewModel model, [FromServices] IWebHostEnvironment host)
        {

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            if (model.EditModel != null)
            {
                if (!string.IsNullOrEmpty(model.EditModel.FirstName))
                {
                    user.FirstName = model.EditModel.FirstName;
                }

                if (!string.IsNullOrEmpty(model.EditModel.LastName))
                {
                    user.LastName = model.EditModel.LastName;
                }

                if (!string.IsNullOrEmpty(model.EditModel.City))
                {
                    user.City = model.EditModel.City;
                }
                if (!string.IsNullOrEmpty(model.EditModel.Email))
                {
                    user.Email = model.EditModel.Email;
                    user.UserName = user.Email;
                }



                if (model.EditModel.ProfileImage != null)
                {
                    string uniqueFileName = ProcessUploadedFile(model.EditModel.ProfileImage, host);
                    user.Image = uniqueFileName;
                }




                if (!string.IsNullOrEmpty(model.EditModel.NewPassword))
                {
                    var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.EditModel.CurrentPassword, model.EditModel.NewPassword);

                    if (!changePasswordResult.Succeeded)
                    {
                        foreach (var error in changePasswordResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }

                        return View("UserDetails", model);
                    }
                }
            }

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("UserDetails");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View("UserDetails", model);
        }

        private string ProcessUploadedFile(IFormFile ProfileImage, IWebHostEnvironment host)
        {
            string uniqueFileName = null;

            if (ProfileImage != null)
            {
                string uploadsFolder = Path.Combine(host.WebRootPath, "User");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ProfileImage.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    ProfileImage.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }







        public async Task<IActionResult> OrderItemsAsync(int id)
        {

            var orderItems = _context.OrderItems
                .Where(oi => oi.OrderId == id)
                .Include(oi => oi.Product)
                .Include(oi => oi.Order)
                .ToList();
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

            return View(orderItems);
        }





    }
}
