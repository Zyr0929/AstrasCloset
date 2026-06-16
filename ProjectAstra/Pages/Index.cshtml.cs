using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectAstra.Models;

namespace ProjectAstra.Pages
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public List<List<ApparelProduct>> ProductPages { get; set; } = new();
        public int TotalPages { get; set; }

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            var trendingProducts = await _context.Products
                .Include(p => p.Variations)
                .Take(4)
                .ToListAsync();

            int itemsPerPage = 4;
            for (int i = 0; i < trendingProducts.Count; i += itemsPerPage)
            {
                ProductPages.Add(trendingProducts.Skip(i).Take(itemsPerPage).ToList());
            }

            TotalPages = ProductPages.Count == 0 ? 1 : ProductPages.Count;
        }
    }
}