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
    public class TestimonialController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TestimonialController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Testimonial
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Testimonial.Include(t => t.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/Testimonial/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Testimonial == null)
            {
                return NotFound();
            }

            var testimonial = await _context.Testimonial
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.TestimonialId == id);
            if (testimonial == null)
            {
                return NotFound();
            }

            return View(testimonial);
        }

        // GET: Admin/Testimonial/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id");
            return View();
        }

        // POST: Admin/Testimonial/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TestimonialId,Name,Email,UserId,TestimonialMessage,isActive")] Testimonial testimonial)
        {
            if (ModelState.IsValid)
            {
                _context.Add(testimonial);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id", testimonial.UserId);
            return View(testimonial);
        }

        // GET: Admin/Testimonial/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Testimonial == null)
            {
                return NotFound();
            }

            var testimonial = await _context.Testimonial.FindAsync(id);
            if (testimonial == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id", testimonial.UserId);
            return View(testimonial);
        }

        // POST: Admin/Testimonial/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TestimonialId,Name,Email,UserId,TestimonialMessage,isActive")] Testimonial testimonial)
        {
            if (id != testimonial.TestimonialId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(testimonial);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TestimonialExists(testimonial.TestimonialId))
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
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id", testimonial.UserId);
            return View(testimonial);
        }

        // GET: Admin/Testimonial/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Testimonial == null)
            {
                return NotFound();
            }

            var testimonial = await _context.Testimonial
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.TestimonialId == id);
            if (testimonial == null)
            {
                return NotFound();
            }

            return View(testimonial);
        }

        // POST: Admin/Testimonial/Delete/5
        // POST: Admin/Testimonials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                if (_context.Testimonial == null)
                {
                    return Problem("Entity set 'ApplicationDbContext.Testimonials' is null.");
                }

                var testimonial = await _context.Testimonial.FindAsync(id);

                if (testimonial == null)
                {
                    return NotFound();
                }

                _context.Testimonial.Remove(testimonial);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Testimonial");
            }
            catch (Exception ex)
            {
                // Log the exception or handle it according to your application's requirements
                return Problem($"An error occurred while deleting the testimonial. {ex.Message}");
            }
        }


        public IActionResult ToggleStatus(int id)
        {
            var review = _context.Testimonial.Find(id);

            if (review != null)
            {
                review.isActive = !review.isActive;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }


        private bool TestimonialExists(int id)
        {
          return (_context.Testimonial?.Any(e => e.TestimonialId == id)).GetValueOrDefault();
        }
    }
}
