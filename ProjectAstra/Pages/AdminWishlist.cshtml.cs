using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectAstra.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectAstra.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminWishlistModel : PageModel
    {
        private readonly AppDbContext _context;
        public AdminWishlistModel(AppDbContext context) => _context = context;

        public class WishlistSummary
        {
            public ApparelProduct Product { get; set; }
            public int TotalWishes { get; set; }
        }

        public List<WishlistSummary> TopWishlist { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Group by ProductId and count them up
            var groupedWishes = await _context.WishlistItems
                .GroupBy(w => w.ProductId)
                .Select(g => new { ProductId = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToListAsync();

            var productIds = groupedWishes.Select(w => w.ProductId).ToList();
            var products = await _context.Products.Include(p => p.Variations)
                .Where(p => productIds.Contains(p.Id)).ToListAsync();

            // Match the counts to the actual product details
            TopWishlist = groupedWishes.Select(w => new WishlistSummary
            {
                Product = products.FirstOrDefault(p => p.Id == w.ProductId),
                TotalWishes = w.Count
            }).Where(w => w.Product != null && !w.Product.IsArchived).ToList();
        }
    }
}