using SmallBusiness.Models;
using SmallBusiness.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Authorization;
using NuGet.Protocol.Plugins;
using MimeKit;
using MailKit.Net.Smtp;
using MimeKit;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;

namespace Business.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;
        public AccountController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;

        }


        public IActionResult Pricing()
        {
            return View();
        }

        #region Register


        public IActionResult RegisterUser()
        {
            var model = new RegisterUserViewModel();
            return View("RegisterUser", model);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser(RegisterUserViewModel model, [FromServices] IWebHostEnvironment host)
        {

            if (ModelState.IsValid)
            {


                
                var user = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserName = model.Email,
                    Email = model.Email,
                    City = model.City,
                    Image = model.Image,
                    EmailConfirmed = false
                };
                

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {

                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);

                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("LEARTS", "learts2024@gmail.com"));
                    message.To.Add(new MailboxAddress($"{user.FirstName} {user.LastName}", user.Email));
                    message.Subject = "Confirm your email";
                    var bodyBuilder = new BodyBuilder();
                    bodyBuilder.HtmlBody = $@"
    <p>Please confirm your email by clicking on the link below:</p>
    <p><a href='{confirmationLink}' style='display:inline-block; padding:10px 20px; background-color:#4CAF50; color:white; text-decoration:none; border-radius:5px;'>Confirm Email</a></p>
    <img src='https://htmldemo.net/learts/learts/assets/images/logo/logo-2.webp' alt='Your Image' width='100' height='100'>
";


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


                    string adminEmail = "zaheraalakash15@gmail.com";
                    if (model.Email.Equals(adminEmail, StringComparison.OrdinalIgnoreCase))
                    {
                        if (!await _roleManager.RoleExistsAsync("Admin"))
                        {
                            await _roleManager.CreateAsync(new IdentityRole("Admin"));
                        }

                        await _userManager.AddToRoleAsync(user, "Admin");
                    }
                    else
                    {
                        if (!await _roleManager.RoleExistsAsync("User"))
                        {
                            await _roleManager.CreateAsync(new IdentityRole("User"));
                        }

                        await _userManager.AddToRoleAsync(user, "User");
                    }
                    ViewBag.ShowSweetAlert = true;
                }


                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }


            return View("RegisterUser", model);
        }


        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }



        public IActionResult RegisterSeller()
        {
            var model = new RegisterUserViewModel();
            return View("RegisterSeller", model);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterSeller(RegisterUserViewModel model)
        
        
        
        {
            if (ModelState.IsValid)
            {

                var seller = new Seller
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserName = model.Email,
                    Email = model.Email,
                    City = model.City,
                    Image = model.Image,
                    EmailConfirmed = false,
                    RegisterDate = DateTime.Now,
                    IsApproved = false
                };

                var result = await _userManager.CreateAsync(seller, model.Password);

                if (result.Succeeded)
                {
                    await AssignRole(seller, "Seller");
                    return RedirectToAction("Profile", "Seller", new { area = "Seller", userId = seller.Id });
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View("RegisterSeller", model);
        }

        private async Task AssignRole(User user, string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }

            await _userManager.AddToRoleAsync(user, roleName);
        }

        #endregion


        //#region Login

        //public IActionResult Login()
        //{
        //    return View("Login");
        //}
        ////[HttpPost]
        ////public async Task<IActionResult> Login(LoginViewModel model)
        ////{
        ////    if (ModelState.IsValid)
        ////    {
        ////        var user = await _userManager.FindByEmailAsync(model.Email);

        ////        if (user != null && !user.EmailConfirmed)
        ////        {
        ////            ModelState.AddModelError(string.Empty, "Please confirm your email before logging in.");
        ////            return View("Login", model);
        ////        }

        ////        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

        ////        if (result.Succeeded)
        ////        {
        ////            HttpContext.Session.SetString("UserId", user.Id);
        ////            HttpContext.Session.SetString("UserName", user.UserName);
        ////            HttpContext.Session.SetString("Email", user.Email);

        ////            var roles = await _userManager.GetRolesAsync(user);

        ////            if (roles.Contains("Admin"))
        ////            {
        ////                return RedirectToAction("Index", "Home", new { area = "Admin" });
        ////            }
        ////            else if (roles.Contains("Seller"))
        ////            {
        ////                return RedirectToAction("Index", "Home", new { area = "Seller" });
        ////            }
        ////            else
        ////            {
        ////                return RedirectToAction("Index", "Home");
        ////            }
        ////        }
        ////        else
        ////        {
        ////            ModelState.AddModelError(string.Empty, "Invalid email or password.");
        ////        }
        ////    }

        ////    return View("Login", model);
        ////}

        //public async Task<IActionResult> Login(LoginViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await _userManager.FindByEmailAsync(model.Email);

        //        if (user != null && !user.EmailConfirmed)
        //        {
        //            ModelState.AddModelError(string.Empty, "Please confirm your email before logging in.");
        //            return View("Login", model);
        //        }

        //        // Check if the user is a seller and is approved
        //        if (user is Seller seller && !seller.IsApproved)
        //        {
        //            ModelState.AddModelError(string.Empty, "Your account is not approved. Please make a payment to activate your account.");
        //            return View("Login", model);
        //        }

        //        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

        //        if (result.Succeeded)
        //        {
        //            HttpContext.Session.SetString("UserId", user.Id);
        //            HttpContext.Session.SetString("UserName", user.UserName);
        //            HttpContext.Session.SetString("Email", user.Email);

        //            var roles = await _userManager.GetRolesAsync(user);

        //            if (roles.Contains("Admin"))
        //            {
        //                return RedirectToAction("Index", "Home", new { area = "Admin" });
        //            }
        //            else if (roles.Contains("Seller"))
        //            {
        //                return RedirectToAction("Index", "Home", new { area = "Seller" });
        //            }
        //            else
        //            {
        //                return RedirectToAction("Index", "Home");
        //            }
        //        }
        //        else
        //        {
        //            ModelState.AddModelError(string.Empty, "Invalid email or password.");
        //        }
        //    }

        //    return View("Login", model);
        //}


        //public async Task<IActionResult> Logout()
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        HttpContext.Session.Clear();

        //        await _signInManager.SignOutAsync();
        //    }

        //    return RedirectToAction("Index", "Home");
        //}

        //#endregion

        #region Login

        public IActionResult Login()
        {
            return View("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null && !user.EmailConfirmed)
                {
                    ModelState.AddModelError(string.Empty, "Please confirm your email before logging in.");
                    return View("Login", model);
                }

                // Check if the user is a seller and is approved
                if (user is Seller seller && !seller.IsApproved)
                {
                    ModelState.AddModelError(string.Empty, "Your account is not approved. Please make a payment to activate your account.");
                    return View("Login", model);
                }
              
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    HttpContext.Session.SetString("UserId", user.Id);
                    HttpContext.Session.SetString("UserName", user.UserName);
                    HttpContext.Session.SetString("Email", user.Email);

                    var roles = await _userManager.GetRolesAsync(user);

                    if (roles.Contains("Admin"))
                    {
                        return RedirectToAction("Index", "Home", new { area = "Admin" });
                    }
                    else if (roles.Contains("Seller"))
                    {
                        return RedirectToAction("Index", "Home", new { area = "Seller" });
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid email or password.");
                }
            }

            return View("Login", model);
        }

        public async Task<IActionResult> Logout()
        {
            if (User.Identity.IsAuthenticated)
            {
                HttpContext.Session.Clear();

                await _signInManager.SignOutAsync();
            }

            return RedirectToAction("Index", "Home");
        }

        #endregion
        #region password
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null && await _userManager.IsEmailConfirmedAsync(user))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, token }, protocol: HttpContext.Request.Scheme);

                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("LEARTS", "learts2024@gmail.com"));
                    message.To.Add(new MailboxAddress($"{user.FirstName} {user.LastName}", user.Email));
                    message.Subject = "Reset your password";

                    var bodyBuilder = new BodyBuilder();
                    bodyBuilder.HtmlBody = $@"
                <p>You have requested to reset your password. Click on the link below to reset it:</p>
                <p><a href='{callbackUrl}' style='display:inline-block; padding:10px 20px; background-color:#4CAF50; color:white; text-decoration:none; border-radius:5px;'>Reset Password</a></p>
                <img src='https://htmldemo.net/learts/learts/assets/images/logo/logo-2.webp' alt='Your Image' width='100' height='100'>
            ";

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

                    return RedirectToAction("Login", "Account"); 
                }

                return RedirectToAction("Login", "Account"); 
            }

            return View(model);
        }


        [HttpGet]
        public IActionResult ResetPassword(string userId, string token)
        {
            var model = new ResetPasswordViewModel { UserId = userId, Token = token };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user != null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Login","Account"); 
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return RedirectToAction("Login", "Account"); 
                }

                return RedirectToAction("Login", "Account"); 
            }

            return View(model);
        }

        #endregion
    }

}
