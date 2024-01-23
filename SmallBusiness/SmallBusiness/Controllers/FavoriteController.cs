using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmallBusiness.Data; // Assuming you have a DbContext named 'SmallBusinessDbContext'
using SmallBusiness.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SmallBusiness.Controllers
{
    public class FavoriteController : Controller
    {
        
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _context;

        public FavoriteController(UserManager<User> userManager,
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
        public async Task<IActionResult> FavoriteListAsync()
        {
            User user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                // Retrieve favorites including associated products
                var favoriteProducts = _context.Favorite
                    .Include(f => f.Product)
                    .Where(f => f.UserId == user.Id && f.IsFav)
                    .ToList();

                ViewBag.Fav = favoriteProducts;
            }
            else
            {
                ViewBag.Fav = new List<Favorite>(); // or handle this case as needed
            }

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

            return View();
        }





        //[HttpPost]
        //public async Task<IActionResult> ToggleFavorite(int productId)
        //{
        //    var product = await _context.Product.FindAsync(productId);

        //    if (product == null)
        //    {
        //        return NotFound();
        //    }

        //    product.IsFav = !product.IsFav;
        //    await _context.SaveChangesAsync();

        //    return Json(new { IsFav = product.IsFav });
        //}



        [HttpPost]
        public async Task<IActionResult> ToggleFavorite(int productId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized(); // You may want to handle this case differently
            }

            var favorite = await _context.Favorite
                .FirstOrDefaultAsync(f => f.ProductId == productId && f.UserId == user.Id);

            if (favorite == null)
            {
                // Product is not in favorites, add it
                _context.Favorite.Add(new Favorite
                {
                    UserId = user.Id,
                    ProductId = productId,
                    IsFav = true // Set IsFav to true when adding to favorites
                });
            }
            else
            {
                // Product is in favorites, toggle the IsFav property
                favorite.IsFav = !favorite.IsFav;
            }

            await _context.SaveChangesAsync();

            // Retrieve user-specific favorites after saving changes
            var userFavorites = await _context.Favorite
                .Where(f => f.UserId == user.Id && f.IsFav)
                .Select(f => f.ProductId)
                .ToListAsync();

            return Json(new { IsFav = userFavorites.Contains(productId), UserFavorites = userFavorites });
        }




    }
}
