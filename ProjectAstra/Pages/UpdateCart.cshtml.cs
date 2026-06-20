using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectAstra.Models;
using System.Threading.Tasks;

namespace ProjectAstra.Pages
{
    [Authorize]
    [IgnoreAntiforgeryToken] // Allows the Layout forms to post safely from any page on the site
    public class UpdateCartModel : PageModel
    {
        private readonly AppDbContext _context;

        public UpdateCartModel(AppDbContext context)
        {
            _context = context;
        }

        public void OnGet() { }

        // HANDLER 1: Handles the [+] and [-] buttons
        public async Task<IActionResult> OnPostUpdateQuantityAsync(int cartItemId, string action, string returnUrl)
        {
            var item = await _context.CartItems.FirstOrDefaultAsync(c => c.Id == cartItemId && c.Username == User.Identity.Name);

            if (item != null)
            {
                if (action == "increase") item.Quantity++;
                else if (action == "decrease") item.Quantity--;

                // Automatically delete the item if quantity drops to 0
                if (item.Quantity <= 0)
                {
                    _context.CartItems.Remove(item);
                }

                await _context.SaveChangesAsync();
            }

            return SafeRedirect(returnUrl);
        }

        // HANDLER 2: Handles the Trash Can button
        public async Task<IActionResult> OnPostRemoveItemAsync(int cartItemId, string returnUrl)
        {
            var item = await _context.CartItems.FirstOrDefaultAsync(c => c.Id == cartItemId && c.Username == User.Identity.Name);

            if (item != null)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
            }

            return SafeRedirect(returnUrl);
        }

        // Utility: Forces the cart drawer to pop back open after the page reloads
        private IActionResult SafeRedirect(string returnUrl)
        {
            string url = string.IsNullOrEmpty(returnUrl) ? "/Index" : returnUrl;

            // Appends the deeplink hook you already have set up in your JS!
            url = url.Contains("?") ? url + "&openCart=true" : url + "?openCart=true";

            return Redirect(url);
        }
    }
}