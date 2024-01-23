using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using SmallBusiness.Data;
using SmallBusiness.Models;
using SmallBusiness.ViewModels;
using System.Security.Claims;

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


        [Authorize]
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

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView("_CartPartial", ViewBag.CartItems);
                }

                return View();
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


            //#region Fav
            //var favoriteProducts = _context.Product.Where(p => p.IsFav).ToList();

            //ViewBag.Fav = favoriteProducts;
            //#endregion

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_CartPartial", ViewBag.CartItems);
            }

            return View();
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

            ViewBag.CartItems = cartItems;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            return RedirectToAction("SuccessfulOrder");
        }

        public async Task<IActionResult> SuccessfulOrder()
        {
            User user = await _userManager.GetUserAsync(User);

            Cart cart = _context.Cart.FirstOrDefault(c => c.UserId == user.Id);

            if (cart == null)
            {
                return NotFound();
            }

            Order order = new Order
            {
                OrderDate = DateTime.Now,
                TotalPrice = cart.TotalPrice,
                Status = "Pending",
                CartId = cart.CartId,
                UserId = user.Id
            };

            _context.Order.Add(order);
            await _context.SaveChangesAsync();


            var cartItems = _context.CartItems.Include(ci => ci.Product).Where(ci => ci.CartId == cart.CartId).ToList();


            ViewBag.CartItems = cartItems;

           



            foreach (var cartitem in cartItems)
            {
                OrderItems orderProduct = new OrderItems();
                orderProduct.ProductId = cartitem.ProductId;
                orderProduct.OrderId = order.OrderId;
                orderProduct.ProductQuantity = cartitem.Quantity;
                _context.Add(orderProduct);
                await _context.SaveChangesAsync();

            }



            Payment payment = new Payment();
            payment.OrderId = order.OrderId;
            payment.Amount = order.TotalPrice;
            payment.PaymentDate = DateTime.Now;
            payment.Status = "paied";
            _context.Add(payment);
            await _context.SaveChangesAsync();


            _context.CartItems.RemoveRange(cartItems);
            cart.TotalPrice = 0;





            await _context.SaveChangesAsync();
            //await SendOrderConfirmationEmail(order, user);

            TempData["SuccessMessage"] = "Your order has been successfully placed. An email confirmation has been sent.";

            TempData["ShowSweetAlert"] = true;

            return View("Checkout");
        }



        public async Task SendOrderConfirmationEmail(Order order, User user)
        {

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("5Dots", "fivedotsoffice@gmail.com"));
            email.To.Add(new MailboxAddress($"{user.FirstName} {user.LastName}", user.Email));
            email.Subject = "Order Confirmation";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.TextBody = $"Dear {user.FirstName} {user.LastName},\n\n"
                             + "Thank you for choosing us!\n"
                             + $"Your order (Order ID: {order.OrderId}) has been successfully placed.\n"
                             + $"Order Total: {order.TotalPrice}JD\n"
                             + "We appreciate your business and look forward to serving you again!\n\n"
                             + "Best regards, Learta Team\n";
            email.Body = bodyBuilder.ToMessageBody();

            using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                smtp.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
                smtp.Connect("smtp.gmail.com", 587, false);
                smtp.Authenticate("fivedotsoffice@gmail.com", "ecjodwaaclzuolqv");
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
            }
        }


    }
}

