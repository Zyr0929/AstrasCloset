using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectAstra.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectAstra.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class EditProductModel : PageModel
    {
        private readonly AppDbContext _context;
        public EditProductModel(AppDbContext context) => _context = context;

        [BindProperty]
        public ApparelProduct Product { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Product = await _context.Products.Include(p => p.Variations).FirstOrDefaultAsync(p => p.Id == id);
            if (Product == null || Product.IsArchived) return RedirectToPage("/Store");
            return Page();
        }

        public async Task<IActionResult> OnPostUpdateAsync()
        {
            var dbProduct = await _context.Products.Include(p => p.Variations).FirstOrDefaultAsync(p => p.Id == Product.Id);
            if (dbProduct == null) return NotFound();

            dbProduct.Name = Product.Name;
            dbProduct.Price = Product.Price;
            dbProduct.Description = Product.Description;
            dbProduct.Category = Product.Category;

            await _context.SaveChangesAsync();
            return RedirectToPage("/Store");
        }

        public async Task<IActionResult> OnPostArchiveAsync(int id)
        {
            var dbProduct = await _context.Products.FindAsync(id);
            if (dbProduct == null) return NotFound();

            var orphanedCartItems = await _context.CartItems.Where(c => c.ProductId == id).ToListAsync();

            foreach (var item in orphanedCartItems)
            {
                bool alreadyWishlisted = await _context.WishlistItems
                    .AnyAsync(w => w.Username == item.Username && w.ProductId == id);

                if (!alreadyWishlisted)
                {
                    _context.WishlistItems.Add(new WishlistItem
                    {
                        Username = item.Username,
                        ProductId = id,
                        DateAdded = DateTime.Now
                    });
                }
            }

            _context.CartItems.RemoveRange(orphanedCartItems);

            dbProduct.IsArchived = true;
            await _context.SaveChangesAsync();

            return RedirectToPage("/Store");
        }
    }
}