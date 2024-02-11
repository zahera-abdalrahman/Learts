using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmallBusiness.Data;
using SmallBusiness.Models;

namespace SmallBusiness.Areas.Seller.Controllers
{
    [Area("Seller")]
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public ReviewController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;

        }
        private string GetCurrentUserId()
        {
            return _userManager.GetUserId(User);
        }

        // GET: Seller/Review
        public async Task<IActionResult> Index()
        {

            var isSubscriptionActive = await CheckSubscriptionStatus();

            if (!isSubscriptionActive)
            {
                TempData["Message"] = "Please renew your subscription to access product listing.";
                return RedirectToAction("Index", "Home"); // Redirect to a page for renewing subscription
            }

            var sellerId = GetCurrentUserId();

            // Retrieve the profile associated with the current seller
            var profile = await _context.Profile.FirstOrDefaultAsync(p => p.SellerId == sellerId);

            if (profile == null)
            {
                return RedirectToAction("CreateProfile"); // Redirect to create a profile if not exists
            }

            // Retrieve the product IDs associated with the current seller's profile
            var productIds = await _context.Product
                .Where(p => p.ProfileId == profile.ProfileId)
                .Select(p => p.ProductId)
                .ToListAsync();

            // Retrieve reviews that belong to products owned by the current seller
            var reviews = await _context.Review
                .Where(r => productIds.Contains(r.ProductId))
                .Include(r => r.Product)
                .Include(r => r.User)
                .ToListAsync();

            return View(reviews);
        }

        private async Task<bool> CheckSubscriptionStatus()
        {
            var sellerId = GetCurrentUserId();

            var subscription = await _context.Subscription.FirstOrDefaultAsync(s => s.SellerId == sellerId);

            return subscription != null && subscription.status;
        }
        // GET: Seller/Review/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Review == null)
            {
                return NotFound();
            }

            var review = await _context.Review
                .Include(r => r.Product)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.ReviewId == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // GET: Seller/Review/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Product, "ProductId", "ImageUrl");
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id");
            return View();
        }

        // POST: Seller/Review/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReviewId,ReviewMessage,ReviewRate,ReviewDate,Name,Email,UserId,ProductId,isActive")] Review review)
        {
            if (ModelState.IsValid)
            {
                _context.Add(review);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Product, "ProductId", "ImageUrl", review.ProductId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id", review.UserId);
            return View(review);
        }

        // GET: Seller/Review/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Review == null)
            {
                return NotFound();
            }

            var review = await _context.Review.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Product, "ProductId", "ImageUrl", review.ProductId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id", review.UserId);
            return View(review);
        }

        // POST: Seller/Review/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReviewId,ReviewMessage,ReviewRate,ReviewDate,Name,Email,UserId,ProductId,isActive")] Review review)
        {
            if (id != review.ReviewId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(review);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReviewExists(review.ReviewId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Product, "ProductId", "ImageUrl", review.ProductId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id", review.UserId);
            return View(review);
        }

        // GET: Seller/Review/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Review == null)
            {
                return NotFound();
            }

            var review = await _context.Review
                .Include(r => r.Product)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.ReviewId == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // POST: Seller/Review/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Review == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Review'  is null.");
            }
            var review = await _context.Review.FindAsync(id);
            if (review != null)
            {
                _context.Review.Remove(review);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReviewExists(int id)
        {
          return (_context.Review?.Any(e => e.ReviewId == id)).GetValueOrDefault();
        }
    }
}
