using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmallBusiness.Data;
using SmallBusiness.Models;
using SmallBusiness.ViewModels;

namespace SmallBusiness.Controllers
{
    public class ShoppingController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public ShoppingController(
            ApplicationDbContext context,
            UserManager<User> userManager
            )
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task<IActionResult> Cart(int ProductId, string change)
        {
            User user = await _userManager.GetUserAsync(User);

            Cart cart = _context.Cart.FirstOrDefault(c => c.UserId == user.Id);

            var isDeleted = false;

            if (change != "add" && change != "delete" && change != "remove")
            {
                var cartitems = _context.CartItems
                    .Include(x => x.Product)
                    .Where(x => x.CartId == cart.CartId)
                    .ToList();

                ViewBag.CartItemsCount = cartitems.Count;
                ViewBag.CartItems = cartitems;

                return View("Cart");
            }


            if (cart == null)
            {
                cart = new Cart { UserId = user.Id, TotalPrice = 0 };

                if (change == "add")
                {
                    _context.Cart.Add(cart);

                    _context.SaveChanges();
                }
            }
            CartItems cartItem = _context
               .CartItems
               .FirstOrDefault(ci => ci.CartId == cart.CartId && ci.ProductId == ProductId);

            if (cartItem == null)
            {
                if (change == "add")
                {
                    cartItem = new CartItems
                    {
                        CartId = cart.CartId,
                        ProductId = ProductId,
                        Quantity = 1
                    };

                    _context.CartItems.Add(cartItem);
                }
            }
            else
            {
                if (change == "add")
                {
                    cartItem.Quantity++;
                }
                else if (change == "remove")
                {
                    if (cartItem.Quantity == 1)
                    {
                        _context.CartItems.Remove(cartItem);
                        _context.SaveChanges();
                        isDeleted = true;
                    }
                    cartItem.Quantity--;
                }
                else if (change == "delete")
                {
                    _context.CartItems.Remove(cartItem);

                    _context.SaveChanges();
                }
            }
            decimal productPrice = (decimal)GetProductPrice(ProductId);
            if (change == "add")
            {
                cart.TotalPrice += productPrice;
            }
            else if (change == "remove" && !isDeleted)
            {
                cart.TotalPrice -= productPrice;
            }

            _context.SaveChanges();

            var cartItemsModified = _context
                .CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.CartId == cart.CartId)
                .ToList();

            ViewBag.CartItems = cartItemsModified;


            return View("Cart");
        }

        private double GetProductPrice(int productId)
        {
            return (double)(_context.Product.FirstOrDefault(p => p.ProductId == productId)?.ProductPrice ?? 0);
        }


        public async Task<IActionResult> Checkout(PaymentViewModel model)
        {
            User user = await _userManager.GetUserAsync(User);

            Cart cart = _context.Cart.FirstOrDefault(c => c.UserId == user.Id);

            var cartItems = _context
                .CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.CartId == cart.CartId)
                .ToList();

            // Pass cart data to the view using ViewBag
            ViewBag.CartItems = cartItems;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            return RedirectToAction("SuccessfulOrder");
        }

        public async Task<IActionResult> SuccessfulOrder()
        {
            // Get the current user
            User user = await _userManager.GetUserAsync(User);

            // Get the cart for the current user
            Cart cart = _context.Cart.FirstOrDefault(c => c.UserId == user.Id);

            if (cart == null)
            {
                // Handle the case when the cart is not found
                return NotFound();
            }

            // Create a new order
            Order order = new Order
            {
                OrderDate = DateTime.Now,
                TotalPrice = cart.TotalPrice,
                Status = "Pending",
                CartId = cart.CartId,
                UserId = user.Id
            };

            // Add the order to the database
            _context.Order.Add(order);

            // Clear the cart
            var cartItems = _context.CartItems.Where(ci => ci.CartId == cart.CartId);
            _context.CartItems.RemoveRange(cartItems);
            cart.TotalPrice = 0;

            // Save the changes
            await _context.SaveChangesAsync();

            return View();
        }

    }
}
