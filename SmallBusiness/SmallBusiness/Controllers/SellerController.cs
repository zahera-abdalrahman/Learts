//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Hosting;
//using SmallBusiness.Data;
//using SmallBusiness.Models;
//using SmallBusiness.ViewModels;

//namespace SmallBusiness.Controllers
//{
//    public class SellerController : Controller
//    {


//        private readonly ApplicationDbContext _context;
//        private readonly UserManager<User> _userManager;

//        public SellerController(ApplicationDbContext context, UserManager<User> userManager)
//        {
//            _context = context;
//            _userManager = userManager;

//        }


//        public IActionResult Index()
//        {

//            return View();
//        }



//        /////////////////////////////////////

//        #region Profile
//        public IActionResult Profile()
//        {
//            var user = _userManager.GetUserAsync(User).Result;

//            var existingProfile = _context.Profile.FirstOrDefault(p => p.SellerId == user.Id);

//            if (existingProfile != null)
//            {
//                return RedirectToAction("EditProfile", new { profileId = existingProfile.ProfileId });
//            }
//            else
//            {
//                return View(); // Render the "Profile" view instead of redirecting
//            }
//        }


//        [HttpPost]
//        public async Task<IActionResult> Profile(ProfileViewModel viewModel, [FromServices] IWebHostEnvironment host)
//        {
//            string ImageName = "";
//            if (viewModel.File != null)
//            {
//                string PathImage = Path.Combine(host.WebRootPath, "Profile");
//                FileInfo fi = new FileInfo(viewModel.File.FileName);
//                ImageName = "Image" + DateTime.UtcNow.ToString().Replace("/", "").Replace(":", "").Replace("-", "").Replace(" ", "") + fi.Extension;

//                string FullPath = Path.Combine(PathImage, ImageName);
//                viewModel.File.CopyTo(new FileStream(FullPath, FileMode.Create));
//            }


//            var user = await _userManager.GetUserAsync(User);
//            var existingProfile = _context.Profile.FirstOrDefault(p => p.SellerId == user.Id);

//            if (existingProfile != null)
//            {
//                return RedirectToAction("EditProfile", new { profileId = existingProfile.ProfileId });
//            }
//            else
//            {
//                var profile = new Profile
//                {
//                    ProfileImage = ImageName,
//                    ShopName = viewModel.ShopName,
//                    Description = viewModel.Description,
//                    IsApproved = viewModel.IsApproved,
//                    SellerId = user.Id,

//                };

//                _context.Profile.Add(profile);
//                _context.SaveChanges();

//            }

//            return View(viewModel);
//        }

//        public IActionResult EditProfile(int profileId)
//        {
//            var profile = _context.Profile.FirstOrDefault(p => p.ProfileId == profileId);

//            if (profile == null || profile.SellerId != GetCurrentUserId())
//            {
//                return RedirectToAction("Profile");
//            }

//            var viewModel = new ProfileViewModel
//            {
//                ProfileId = profile.ProfileId,
//                ProfileImage = profile.ProfileImage,
//                ShopName = profile.ShopName,
//                Description = profile.Description,
//                IsApproved = profile.IsApproved
//            };

//            return View(viewModel);
//        }

      
//        [HttpPost]
//        public IActionResult EditProfile(int profileId, ProfileViewModel viewModel)
//        {
//            if (ModelState.IsValid)
//            {
//                var profile = _context.Profile.FirstOrDefault(p => p.ProfileId == profileId);

//                if (profile == null || profile.SellerId != GetCurrentUserId())
//                {
//                    return RedirectToAction("Profile");
//                }

//                profile.ProfileImage = viewModel.ProfileImage;
//                profile.ShopName = viewModel.ShopName;
//                profile.Description = viewModel.Description;
//                profile.IsApproved = viewModel.IsApproved;

//                _context.Profile.Update(profile);
//                _context.SaveChanges();

//                return RedirectToAction("Profile");
//            }

//            return View(viewModel);
//        }

//        private string GetCurrentUserId()
//        {
//            return _userManager.GetUserId(User);
//        }
//        #endregion

//        /////////////////////////////////////

//        #region AddProduct
//        public IActionResult CreateProduct()
//        {
//            ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "CategoryName");
//            return View();
//        }

//        [HttpPost]
//        public IActionResult CreateProduct(ProductViewModel viewModel, [FromServices] IWebHostEnvironment host)
//        {


//            string ImageName = "";
//            if (viewModel.File != null)
//            {
//                string PathImage = Path.Combine(host.WebRootPath, "CategoryImg");
//                FileInfo fi = new FileInfo(viewModel.File.FileName);
//                ImageName = "Image" + DateTime.UtcNow.ToString().Replace("/", "").Replace(":", "").Replace("-", "").Replace(" ", "") + fi.Extension;

//                string FullPath = Path.Combine(PathImage, ImageName);
//                viewModel.File.CopyTo(new FileStream(FullPath, FileMode.Create));
//            }


//                var sellerId = GetCurrentUserId();
//                var profile = _context.Profile.FirstOrDefault(p => p.SellerId == sellerId);

//                if (profile == null)
//                {
//                    return RedirectToAction("Profile");
//                }


//                var product = new Product
//                {
//                    ProductName = viewModel.ProductName,
//                    ProductDescription = viewModel.ProductDescription,
//                    ProductPrice = viewModel.ProductPrice,
//                    ProductQuantityStock = viewModel.ProductQuantityStock,
//                    ProductSale = viewModel.ProductSale,
//                    ImageUrl = ImageName,
//                    CategoryId = viewModel.CategoryId,
//                    ProfileId = profile.ProfileId,
//                    CreateAt= DateTime.UtcNow,
//                    ReviewRate=viewModel.ReviewRate,
//                };

//                _context.Product.Add(product);
//                _context.SaveChanges();

//                return RedirectToAction("Profile");
//        }


//        #endregion
//    }
//}
