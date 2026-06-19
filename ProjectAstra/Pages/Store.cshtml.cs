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

        public List<string> DbTypes { get; set; } = new();
        public List<string> DbColors { get; set; } = new();
        public List<string> DbSizes { get; set; } = new();
        public List<string> DbThemes { get; set; } = new();

        [BindProperty(SupportsGet = true)] public List<string> Types { get; set; } = new();
        [BindProperty(SupportsGet = true)] public List<string> Colors { get; set; } = new();
        [BindProperty(SupportsGet = true)] public List<string> Sizes { get; set; } = new();
        [BindProperty(SupportsGet = true)] public List<string> Prices { get; set; } = new();
        [BindProperty(SupportsGet = true)] public List<string> Themes { get; set; } = new();
        [BindProperty(SupportsGet = true)] public List<string> Materials { get; set; } = new();

        public async Task OnGetAsync()
        {
            var allProducts = await _context.Products.Include(p => p.Variations).ToListAsync();

            DbTypes = allProducts.SelectMany(p => p.Types ?? new List<string>()).Distinct().OrderBy(t => t).ToList();
            DbSizes = allProducts.SelectMany(p => p.AvailableSizes ?? new List<string>()).Distinct().OrderBy(s => s).ToList();
            DbColors = allProducts.SelectMany(p => p.Variations.Select(v => v.ColorName)).Distinct().OrderBy(c => c).ToList();
            DbThemes = allProducts.Where(p => !string.IsNullOrEmpty(p.Tag)).Select(p => p.Tag).Distinct().OrderBy(t => t).ToList();

            var query = _context.Products.Include(p => p.Variations).AsQueryable();
            var products = await query.ToListAsync();

            if (Types.Any())
            {
                products = products.Where(p => p.Types != null && p.Types.Any(t => Types.Contains(t))).ToList();
            }

            if (Colors.Any())
            {
                var lowerColors = Colors.Select(c => c.ToLower()).ToList();
                products = products.Where(p => p.Variations.Any(v => lowerColors.Contains(v.ColorName.ToLower()))).ToList();
            }

            if (Themes.Any())
            {
                var lowerThemes = Themes.Select(t => t.ToLower()).ToList();
                products = products.Where(p => !string.IsNullOrEmpty(p.Tag) && lowerThemes.Any(t => p.Tag.ToLower().Contains(t))).ToList();
            }

            if (Sizes.Any())
            {
                products = products.Where(p => p.AvailableSizes != null && p.AvailableSizes.Any(s => Sizes.Contains(s))).ToList();
            }

            if (Prices.Any())
            {
                var filteredByPrice = new List<ApparelProduct>();
                if (Prices.Contains("0-350")) filteredByPrice.AddRange(products.Where(p => p.Price <= 350));
                if (Prices.Contains("351-500")) filteredByPrice.AddRange(products.Where(p => p.Price > 350 && p.Price <= 500));
                if (Prices.Contains("500+")) filteredByPrice.AddRange(products.Where(p => p.Price > 500));
                products = filteredByPrice.Distinct().ToList();
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