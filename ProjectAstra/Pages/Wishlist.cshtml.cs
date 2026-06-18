using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectAstra.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectAstra.Pages
{
    public class WishlistModel : PageModel
    {
        private readonly AppDbContext _context;

        public WishlistModel(AppDbContext context)
        {
            _context = context;
        }

        public List<WishlistItem> WishlistItems { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Login");
            }

            WishlistItems = await _context.WishlistItems
                .Include(w => w.Product)
                .ThenInclude(p => p.Variations)
                .Where(w => w.Username == User.Identity.Name)
                .OrderByDescending(w => w.DateAdded)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostRemoveAsync(int id)
        {
            var item = await _context.WishlistItems.FindAsync(id);

            if (item != null && item.Username == User.Identity?.Name)
            {
                _context.WishlistItems.Remove(item);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}