using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore; 
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectAstra.Models;

namespace ProjectAstra.Pages
{
    public class StoreModel : PageModel
    {
        private readonly AppDbContext _context;

        public List<ApparelProduct> StoreProducts { get; set; } = new();

        public StoreModel(AppDbContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {

            StoreProducts = await _context.Products 
                .Include(p => p.Variations)
                .ToListAsync();
        }
    }
}