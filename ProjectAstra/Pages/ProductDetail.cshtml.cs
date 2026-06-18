using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectAstra.Models;
using System.Threading.Tasks;

namespace ProjectAstra.Pages
{
    public class ProductDetailModel : PageModel
    {
        private readonly AppDbContext _context;

        public ProductDetailModel(AppDbContext context)
        {
            _context = context;
        }

        public ApparelProduct Product { get; set; }

        [BindProperty] public string SelectedColor { get; set; }
        [BindProperty] public string SelectedSize { get; set; }
        [BindProperty] public int Quantity { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Product = await _context.Products
                .Include(p => p.Variations)
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (Product == null) return RedirectToPage("/Store");
            return Page();
        }
        public async Task<IActionResult> OnPostAddToWishlistAsync(int id)
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated) return RedirectToPage("/Login");

            var exists = await _context.WishlistItems.AnyAsync(w => w.Username == User.Identity.Name && w.ProductId == id);

            if (!exists)
            {
                _context.WishlistItems.Add(new WishlistItem { Username = User.Identity.Name, ProductId = id });
                await _context.SaveChangesAsync();
            }

            return Redirect($"/Store/ProductDetail/{id}");
        }
        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Login");
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null) return RedirectToPage("/Store");

            var cartItem = new CartItem
            {
                Username = User.Identity.Name,
                ProductId = id,
                SelectedColor = SelectedColor ?? "Default", 
                SelectedSize = SelectedSize ?? "Default",
                Quantity = Quantity > 0 ? Quantity : 1,
                UnitPrice = product.Price
            };

            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();

            return Redirect($"/Store/ProductDetail/{id}?openCart=true");
        }
    }
}