using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmallBusiness.Data;
using SmallBusiness.Models;
using SmallBusiness.ViewModels;

namespace SmallBusiness.Areas.Seller.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class AdminEditController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _context;

        public AdminEditController(UserManager<User> userManager,
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
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return View();
            }


            var UserDetails = new UpdateViewModel
            {

                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                City = user.City,
                CurrentPassword = "",
                NewPassword = "",
                ConfirmPassword = ""
            };
            return View(UserDetails);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(UpdateViewModel model, [FromServices] IWebHostEnvironment host)
        {

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            if (model != null)
            {
                if (!string.IsNullOrEmpty(model.FirstName))
                {
                    user.FirstName = model.FirstName;
                }

                if (!string.IsNullOrEmpty(model.LastName))
                {
                    user.LastName = model.LastName;
                }

                if (!string.IsNullOrEmpty(model.City))
                {
                    user.City = model.City;
                }
                if (!string.IsNullOrEmpty(model.Email))
                {
                    user.Email = model.Email;
                    user.UserName = user.Email;
                }



                if (model.ProfileImage != null)
                {
                    // Process and save the uploaded image
                    string uniqueFileName = ProcessUploadedFile(model.ProfileImage, host);
                    user.Image = uniqueFileName;
                }




                if (!string.IsNullOrEmpty(model.NewPassword))
                {
                    var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

                    if (!changePasswordResult.Succeeded)
                    {
                        foreach (var error in changePasswordResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }

                        return View("Index", model);
                    }
                }
            }

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View("Index", model);
        }

        private string ProcessUploadedFile(IFormFile profileImage, IWebHostEnvironment host)
        {
            string uniqueFileName = null;

            if (profileImage != null)
            {
                string uploadsFolder = Path.Combine(host.WebRootPath, "User");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(profileImage.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    profileImage.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }
    }
}
