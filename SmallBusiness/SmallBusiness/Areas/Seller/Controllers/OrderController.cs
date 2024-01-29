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
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public OrderController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;

        }
        private string GetCurrentUserId()
        {
            return _userManager.GetUserId(User);
        }

        //// GET: Seller/Order
        //public async Task<IActionResult> Index()
        //{
        //    var sellerId = GetCurrentUserId();

        //    // Retrieve the profile associated with the current seller
        //    var profile = await _context.Profile.FirstOrDefaultAsync(p => p.SellerId == sellerId);

        //    if (profile == null)
        //    {
        //        return RedirectToAction("CreateProfile"); // Redirect to create a profile if not exists
        //    }

        //    // Retrieve orders with order items that belong to products owned by the current seller
        //    var orders = await _context.Order
        //        .Include(o => o.OrderItems)
        //        .ThenInclude(oi => oi.Product)
        //        .Where(o => o.OrderItems.Any(oi => oi.Product.ProfileId == profile.ProfileId))
        //        .ToListAsync();

        //    return View(orders);
        //}



        //public async Task<IActionResult> UserOrderItems(int id)
        //{
        //    var userId = GetCurrentUserId();

        //    var sellerProfile = await _context.Profile.FirstOrDefaultAsync(p => p.SellerId == userId);

        //    if (sellerProfile == null)
        //    {
               
        //        return View("Index");
        //    }

        //    // Retrieve the order items for the current user and specified order.
        //    var orderItems = await _context.OrderItems
        //        .Where(oi => oi.Order.User.Id == sellerProfile.SellerId && oi.OrderId == id)
        //        .Include(oi => oi.Product)
        //        .Include(oi=>oi.Order)
        //        .ToListAsync();

        //    if (orderItems == null || !orderItems.Any())
        //    {
        //        // Return a specific view indicating that there are no order items for the specified order.
        //        return View("Index");
        //    }

        //    // Filter the order items to display only the products that belong to the logged-in seller.
        //    var sellerOrderItems = orderItems.Where(oi => oi.Product.ProfileId == sellerProfile.ProfileId);

        //    return View(sellerOrderItems);
        //}





        public async Task<IActionResult> Index()
        {
            var sellerId = GetCurrentUserId();

            // Retrieve the profile associated with the current seller
            var profile = await _context.Profile.FirstOrDefaultAsync(p => p.SellerId == sellerId);

            if (profile == null)
            {
                return RedirectToAction("CreateProfile"); // Redirect to create a profile if not exists
            }

            // Retrieve orders with order items that belong to products owned by the current seller
            var orders = await _context.Order
                .Include(o=>o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.OrderItems.Any(oi => oi.Product.ProfileId == profile.ProfileId))
                .ToListAsync();

            return View(orders);
        }

        // GET: Seller/Order/UserOrderItems/5
        public async Task<IActionResult> UserOrderItems(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sellerId = GetCurrentUserId();

            // Retrieve the profile associated with the current seller
            var profile = await _context.Profile.FirstOrDefaultAsync(p => p.SellerId == sellerId);

            if (profile == null)
            {
                return RedirectToAction("Index"); // Redirect to create a profile if not exists
            }

            // Retrieve the specific order with order items that belong to products owned by the current seller
            var order = await _context.Order
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderId == id && o.OrderItems.Any(oi => oi.Product.ProfileId == profile.ProfileId));

            if (order == null)
            {
                return NotFound();
            }

            // Filter the order items based on the seller's profile ID
            var sellerOrderItems = order.OrderItems.Where(oi => oi.Product.ProfileId == profile.ProfileId).ToList();

            return View(sellerOrderItems);
        }


        // GET: Seller/Order/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.Cart)
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Seller/Order/Create
        public IActionResult Create()
        {
            ViewData["CartId"] = new SelectList(_context.Cart, "CartId", "UserId");
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id");
            return View();
        }

        // POST: Seller/Order/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,TotalPrice,Status,OrderDate,CartId,UserId")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CartId"] = new SelectList(_context.Cart, "CartId", "UserId", order.CartId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id", order.UserId);
            return View(order);
        }

        // GET: Seller/Order/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["CartId"] = new SelectList(_context.Cart, "CartId", "UserId", order.CartId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id", order.UserId);
            return View(order);
        }

        // POST: Seller/Order/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,TotalPrice,Status,OrderDate,CartId,UserId")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
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
            ViewData["CartId"] = new SelectList(_context.Cart, "CartId", "UserId", order.CartId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id", order.UserId);
            return View(order);
        }

        // GET: Seller/Order/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.Cart)
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Seller/Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Order == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Order'  is null.");
            }
            var order = await _context.Order.FindAsync(id);
            if (order != null)
            {
                _context.Order.Remove(order);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
          return (_context.Order?.Any(e => e.OrderId == id)).GetValueOrDefault();
        }
    }
}
