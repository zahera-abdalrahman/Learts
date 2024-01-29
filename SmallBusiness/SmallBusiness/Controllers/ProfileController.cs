using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MimeKit;
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


        public async Task<IActionResult> AllSellersAsync()
        {
            var allSellers = _context.Profile
                .Where(p => p.SellerId != null)
                .Include(p => p.Seller)
                .Where(p=>p.Seller.IsApproved==true)
                .ToList();

            ViewBag.AllSellers = allSellers;
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



            //#region Fav
            //var favoriteProducts = _context.Product.Where(p => p.IsFav).ToList();

            //ViewBag.Fav = favoriteProducts;
            //#endregion
            return View();
        }


        public async Task<IActionResult> ProfileDetailsAsync(int profileId)
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

            #region Fav
            var favoriteProducts = _context.Favorite.Include(c => c.Product).Where(p => p.IsFav).ToList();

            ViewBag.Fav = favoriteProducts;
            #endregion


            return View();
        }

        private string GetSellerEmailForProfileId(int profileId)
        {
            var sellerEmail = _context.Profile
                .Where(p => p.ProfileId == profileId)
                .Select(p => p.Seller.Email)
                .FirstOrDefault();

            return sellerEmail;
        }

        [HttpPost]
        public IActionResult SendMessage(ContactProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                int profileId = model.ProfileId;

                SendEmailAsync(model, profileId);

                ViewBag.SuccessMessage = "Your message has been sent successfully!";
                return RedirectToAction("ProfileDetails", new { profileId = profileId });
            }

            return View("ProfileDetails", model);
        }


        private async Task SendEmailAsync(ContactProfileViewModel model, int profileId)
        {
            var sellerEmail = GetSellerEmailForProfileId(profileId);

            if (sellerEmail == null)
            {
                return;
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(model.Name, model.Email));
            message.To.Add(new MailboxAddress("Seller", sellerEmail)); 
            message.Subject = "New Contact Form Submission";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = $@"
    <p>You have received a new contact form submission:</p>
    <p><strong>Name:</strong> {model.Name}</p>
    <p><strong>Email:</strong> {model.Email}</p>
    <p><strong>Message:</strong> {model.Message}</p>";

            message.Body = bodyBuilder.ToMessageBody();

            var smtpServer = "smtp.gmail.com";
            var smtpPort = 587;
            var useSsl = false;
            var smtpUsername = "learts2024@gmail.com";
            var smtpPassword = "luuqmyopbrmchuez";
            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                await client.ConnectAsync(smtpServer, smtpPort, useSsl);
                await client.AuthenticateAsync(smtpUsername, smtpPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}