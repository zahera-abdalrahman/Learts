﻿using MailKit.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using SmallBusiness.Data;
using SmallBusiness.Models;
using SmallBusiness.ViewModels;
using System.Security.Claims;

namespace SmallBusiness.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _context;

        public AdminController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _context = context;

        }

       

        #region Users
        public IActionResult GetUsers(string searchTerm)
        {
            var usersWithUserRole = _userManager.GetUsersInRoleAsync("User").Result;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                // Filter users based on the search term
                usersWithUserRole = usersWithUserRole
                    .Where(u => u.FirstName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                u.LastName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            ViewBag.Users = usersWithUserRole;
            ViewBag.Role = "User";
            ViewBag.SearchTerm = searchTerm;

            return View("GetUsers");
        }


        public IActionResult UserDetails(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }

            var user = _userManager.FindByIdAsync(userId).Result;

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }


        [HttpPost]
        public IActionResult Delete(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }

            var userToDelete = _userManager.FindByIdAsync(userId).Result;

            if (userToDelete == null)
            {
                return NotFound();
            }

            var result = _userManager.DeleteAsync(userToDelete).Result;

            if (result.Succeeded)
            {
                // Successfully deleted the user
                return RedirectToAction("GetUsers");
            }
            else
            {
                // Handle the error appropriately
                return View("Error");
            }
        }
        #endregion

        #region Sellers
        public IActionResult GetSellers(string searchTerm)
        {

            var usersWithSellerRole = _userManager.GetUsersInRoleAsync("Seller").Result;

            var sellersWithProfiles = usersWithSellerRole
                .Select(user => new
                {
                    Seller = user,
                    Profile = _context.Profile.FirstOrDefault(p => p.SellerId == user.Id)
                });


            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                sellersWithProfiles = sellersWithProfiles
                .Where(seller =>
                        seller.Seller.FirstName.ToLower().Contains(searchTerm) ||
                        seller.Seller.LastName.ToLower().Contains(searchTerm)
                    );
            }



            ViewBag.SellersWithProfiles = sellersWithProfiles;
            ViewBag.Role = "Seller";

            return View("GetSellers");
        }
        public async Task<IActionResult> ToggleStatusAsync(string id)
        {
            var user = _context.Seller.Find(id);

            if (user != null)
            {
                user.IsApproved = !user.IsApproved;
                _context.SaveChanges();

                if (user.IsApproved)
                {
                    var confirmationLink = Url.Action("ConfirmEmail", "Account", new { area = "", userId = user.Id, token = await _userManager.GenerateEmailConfirmationTokenAsync(user) }, Request.Scheme);


                    await SendActivationEmail(user.Email, user.FirstName, user.LastName, confirmationLink);
                }
            }

            return RedirectToAction("GetSellers");
        }

        private async Task SendActivationEmail(string userEmail, string firstName, string lastName, string confirmationLink)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("LEARTS", "learts2024@gmail.com"));
            message.To.Add(new MailboxAddress($"{firstName} {lastName}", userEmail));
            message.Subject = "Account Activated";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = $@"
        <p>Your account has been activated successfully.</p>
        <p>Thank you for choosing our service.</p>
        <p>Please confirm your email by clicking on the link below:</p>
        <p><a href='{confirmationLink}' style='display:inline-block; padding:10px 20px; background-color:#4CAF50; color:white; text-decoration:none; border-radius:5px;'>Confirm Email</a></p>
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
        }


        public IActionResult SellerDetails(string sellerId)
        {


            if (string.IsNullOrEmpty(sellerId))
            {
                return NotFound();
            }

            var seller = _context.Seller.FirstOrDefault(s => s.Id == sellerId);

            if (seller == null)
            {
                return NotFound();
            }

            return View(seller);
        }


        public IActionResult ProfileDetails(string sellerId)
        {

            var seller = _context.Seller.FirstOrDefault(s => s.Id == sellerId);

            if (seller == null)
            {
                return RedirectToAction("GetSellers");
            }

            var profile = _context.Profile.FirstOrDefault(p => p.SellerId == sellerId);

            if (profile == null)
            {
                return RedirectToAction("GetSellers");
            }

            return View(profile);
        }




        #endregion
    }
}