using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmallBusiness.Data;
using SmallBusiness.Models;
using System.Security.Claims;

namespace SmallBusiness.Controllers
{
    public class FavoriteQuantityViewComponent : ViewComponent
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public FavoriteQuantityViewComponent(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                User user = await _userManager.GetUserAsync((ClaimsPrincipal)User);

                if (user != null)
                {
                    // Assuming you have a property named 'IsFav' in your Product model
                    var favoriteQuantity = _context.Favorite
                        .Count(p => p.IsFav && p.UserId == user.Id);

                    return View(favoriteQuantity);
                }
            }

            return View(0);
        }
    }
}
