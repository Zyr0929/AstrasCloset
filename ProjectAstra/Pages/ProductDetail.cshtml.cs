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

            // Safety check: Prevent users from loading an archived product directly via URL
            if (Product == null || Product.IsArchived) return RedirectToPage("/Store");

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

            var currentUser = User.Identity.Name;

            // Normalize inputs to ensure we don't accidentally save null values
            var safeColor = SelectedColor ?? "Default";
            var safeSize = SelectedSize ?? "Default";
            var safeQty = Quantity > 0 ? Quantity : 1;

            // 1. SMART CHECK: Look for this exact item in the user's cart
            var existingItem = await _context.CartItems.FirstOrDefaultAsync(c =>
                c.Username == currentUser &&
                c.ProductId == id &&
                c.SelectedColor == safeColor &&
                c.SelectedSize == safeSize);

            if (existingItem != null)
            {
                // 2. CONSOLIDATE: Just add the new quantity to the existing row
                existingItem.Quantity += safeQty;
            }
            else
            {
                // 3. CREATE: If it doesn't exist, create a brand new entry
                var newCartItem = new CartItem
                {
                    Username = currentUser,
                    ProductId = id,
                    SelectedColor = safeColor,
                    SelectedSize = safeSize,
                    Quantity = safeQty,
                    UnitPrice = product.Price
                };

                _context.CartItems.Add(newCartItem);
            }

            await _context.SaveChangesAsync();

            // Redirect back to the page and trigger the cart drawer to open
            return Redirect($"/Store/ProductDetail/{id}?openCart=true");
        }
    }
}