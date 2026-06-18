using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectAstra.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectAstra.Pages
{
    public class SettingsModel : PageModel
    {
        private readonly AppDbContext _context;

        public SettingsModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string CurrentPhase { get; set; }

        [BindProperty]
        public DateTime PreorderDeadline { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public void OnGet()
        {
            var setting = _context.StoreSettings.FirstOrDefault();

            if (setting != null)
            {
                CurrentPhase = setting.CurrentPhase;
                PreorderDeadline = setting.PreorderDeadline;
            }
            else
            {
                CurrentPhase = "Preorder";
                PreorderDeadline = DateTime.Now.AddDays(3);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var setting = await _context.StoreSettings.FirstOrDefaultAsync();

            if (setting != null)
            {
                bool wasPreorder = setting.CurrentPhase == "Preorder";
                bool isClosingPreorders = CurrentPhase != "Preorder";

                if (wasPreorder && isClosingPreorders)
                {
                    var allCartItems = await _context.CartItems.ToListAsync();

                    if (allCartItems.Any())
                    {
                        foreach (var cartItem in allCartItems)
                        {
                            bool alreadyInWishlist = await _context.WishlistItems
                                .AnyAsync(w => w.Username == cartItem.Username && w.ProductId == cartItem.ProductId);

                            if (!alreadyInWishlist)
                            {
                                _context.WishlistItems.Add(new WishlistItem
                                {
                                    Username = cartItem.Username,
                                    ProductId = cartItem.ProductId,
                                    DateAdded = DateTime.Now
                                });
                            }
                        }

                        _context.CartItems.RemoveRange(allCartItems);
                    }
                }

                setting.CurrentPhase = CurrentPhase;
                setting.PreorderDeadline = PreorderDeadline;

                await _context.SaveChangesAsync();

                StatusMessage = "Store phase updated! All abandoned carts were automatically converted to wishlists.";
            }

            return RedirectToPage();
        }
    }
}