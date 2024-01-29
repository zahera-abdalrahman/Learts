using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using SmallBusiness.Data;
using SmallBusiness.Models;
using SmallBusiness.ViewModels;

namespace SmallBusiness.Controllers
{
    public class ContactController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _context;

        public ContactController(UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<User> signInManager,
            ILogger<ProductController> logger,
            ApplicationDbContext context
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> ContactAsync()
        {
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

            return View();
        }

        [HttpPost]
        public IActionResult SendMessage(ContactFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Data Not Completed");
                return View();
            }

            SendEmailAsync(model);

            return Json(new { success = true });
        }

        private async Task SendEmailAsync(ContactFormViewModel model)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(model.Name, model.Email));
            message.To.Add(new MailboxAddress("Admin", "zaheraalakash15@gmail.com"));
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
