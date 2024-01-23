using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmallBusiness.Data;
using SmallBusiness.Models;
using System.Security.Claims;

namespace SmallBusiness.Controllers
{
    public class CartQuantityViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public CartQuantityViewComponent(ApplicationDbContext context, UserManager<User> userManager)
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
                    Cart cart = _context.Cart.FirstOrDefault(c => c.UserId == user.Id);

                    if (cart != null)
                    {
                        var totalQuantity = _context.CartItems
                            .Where(ci => ci.CartId == cart.CartId)
                            .Sum(ci => ci.Quantity);

                        return View(totalQuantity);
                    }
                }
            }

            return View(0);
        }
    }
}
