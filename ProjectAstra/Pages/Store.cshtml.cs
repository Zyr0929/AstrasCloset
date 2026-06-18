using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectAstra.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectAstra.Pages
{
    public class StoreModel : PageModel
    {
        private readonly AppDbContext _context;

        public StoreModel(AppDbContext context)
        {
            _context = context;
        }

        public List<ApparelProduct> StoreProducts { get; set; } = new();

        // 1. Catch all active filters from the URL
        [BindProperty(SupportsGet = true)] public List<string> Types { get; set; } = new();
        [BindProperty(SupportsGet = true)] public List<string> Colors { get; set; } = new();
        [BindProperty(SupportsGet = true)] public List<string> Sizes { get; set; } = new();
        [BindProperty(SupportsGet = true)] public List<string> Prices { get; set; } = new();
        [BindProperty(SupportsGet = true)] public List<string> Themes { get; set; } = new();
        [BindProperty(SupportsGet = true)] public List<string> Materials { get; set; } = new();

        public async Task OnGetAsync()
        {
            var query = _context.Products.Include(p => p.Variations).AsQueryable();

            if (Types.Any())
            {
                query = query.Where(p => Types.Contains(p.Category));
            }

            if (Colors.Any())
            {
                var lowerColors = Colors.Select(c => c.ToLower()).ToList();
                query = query.Where(p => p.Variations.Any(v => lowerColors.Contains(v.ColorName.ToLower())));
            }

            if (Themes.Any())
            {
                var lowerThemes = Themes.Select(t => t.ToLower()).ToList();
                query = query.Where(p => !string.IsNullOrEmpty(p.Tag) && lowerThemes.Any(t => p.Tag.ToLower().Contains(t)));
            }

            var products = await query.ToListAsync();

            if (Prices.Any())
            {
                var filteredByPrice = new List<ApparelProduct>();
                if (Prices.Contains("0-350")) filteredByPrice.AddRange(products.Where(p => p.Price <= 350));
                if (Prices.Contains("351-500")) filteredByPrice.AddRange(products.Where(p => p.Price > 350 && p.Price <= 500));
                if (Prices.Contains("500+")) filteredByPrice.AddRange(products.Where(p => p.Price > 500));
                products = filteredByPrice.Distinct().ToList();
            }

            if (Sizes.Any())
            {
                products = products.Where(p => p.AvailableSizes.Any(s => Sizes.Contains(s))).ToList();
            }

            if (Materials.Any())
            {
                var lowerMats = Materials.Select(m => m.ToLower()).ToList();
                products = products.Where(p => !string.IsNullOrEmpty(p.Description) && lowerMats.Any(m => p.Description.ToLower().Contains(m))).ToList();
            }

            StoreProducts = products;
        }
    }
}