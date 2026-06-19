using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectAstra.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectAstra.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class ProductsManagementModel : PageModel
    {
        private readonly AppDbContext _context;

        public ProductsManagementModel(AppDbContext context)
        {
            _context = context;
        }

        public List<ApparelProduct> Products { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string CurrentFilter { get; set; } = "Active";

        [TempData]
        public string StatusMessage { get; set; }

        public async Task OnGetAsync()
        {
            var query = _context.Products.Include(p => p.Variations).AsQueryable();

            // Apply quick-sorting filters from the UI tabs
            query = CurrentFilter switch
            {
                "Active" => query.Where(p => !p.IsArchived),
                "Archived" => query.Where(p => p.IsArchived),
                "T-Shirt" => query.Where(p => !p.IsArchived && p.Types.Contains("T-Shirt")),
                "Hoodie" => query.Where(p => !p.IsArchived && p.Types.Contains("Hoodie")),
                "ToteBag" => query.Where(p => !p.IsArchived && p.Types.Contains("Tote Bag")),
                _ => query // "All" selection loads everything
            };

            Products = await query.OrderBy(p => p.Name).ToListAsync();
        }

        // HANDLER 1: ARCHIVE AUTOMATION
        public async Task<IActionResult> OnPostArchiveAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            // Find all instances of this product across active student shopping carts
            var cartItems = await _context.CartItems.Where(c => c.ProductId == id).ToListAsync();
            int migratedCount = 0;

            foreach (var item in cartItems)
            {
                bool existsInWishlist = await _context.WishlistItems
                    .AnyAsync(w => w.Username == item.Username && w.ProductId == id);

                if (!existsInWishlist)
                {
                    _context.WishlistItems.Add(new WishlistItem
                    {
                        Username = item.Username,
                        ProductId = id,
                        DateAdded = DateTime.Now
                    });
                    migratedCount++;
                }
            }

            // Wipe items from carts to prevent invalid checkouts
            _context.CartItems.RemoveRange(cartItems);

            product.IsArchived = true;
            await _context.SaveChangesAsync();

            StatusMessage = $"Product '{product.Name}' archived successfully. Removed from shopping carts and converted into {migratedCount} user wishlists.";
            return RedirectToPage(new { CurrentFilter });
        }

        // HANDLER 2: UNARCHIVE RESTORATION
        public async Task<IActionResult> OnPostUnarchiveAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            product.IsArchived = false;
            await _context.SaveChangesAsync();

            StatusMessage = $"Product '{product.Name}' restored to active catalog listing.";
            return RedirectToPage(new { CurrentFilter = "Archived" });
        }
    }
}