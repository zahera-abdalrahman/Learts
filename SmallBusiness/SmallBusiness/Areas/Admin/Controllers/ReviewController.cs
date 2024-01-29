using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmallBusiness.Data;
using SmallBusiness.Models;

namespace SmallBusiness.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReviewController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Review
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Review.Include(r => r.Product).Include(r => r.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/Review/Details/5
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

        // GET: Admin/Review/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Product, "ProductId", "ImageUrl");
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id");
            return View();
        }

        // POST: Admin/Review/Create
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

        // GET: Admin/Review/Edit/5
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

        // POST: Admin/Review/Edit/5
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

        // GET: Admin/Review/Delete/5
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

        // POST: Admin/Review/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                if (_context.Review == null)
                {
                    return Problem("Entity set 'ApplicationDbContext.Review' is null.");
                }

                var review = await _context.Review.FindAsync(id);

                if (review == null)
                {
                    return NotFound();
                }

                _context.Review.Remove(review);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Review");
            }
            catch (Exception ex)
            {
                // Log the exception or handle it according to your application's requirements
                return Problem($"An error occurred while deleting the review. {ex.Message}");
            }
        }







        public IActionResult ToggleStatus(int id)
        {
            var review = _context.Review.Find(id);

            if (review != null)
            {
                review.isActive = !review.isActive;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        private bool ReviewExists(int id)
        {
          return (_context.Review?.Any(e => e.ReviewId == id)).GetValueOrDefault();
        }
    }
}
