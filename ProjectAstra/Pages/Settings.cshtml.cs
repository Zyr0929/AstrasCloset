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
                // -- AUTOMATION 1: CLOSING PRE-ORDERS --
                bool wasPreorder = setting.CurrentPhase == "Preorder";
                bool isClosingPreorders = CurrentPhase != "Preorder";
                bool cartsWiped = false;

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
                        cartsWiped = true;
                    }
                }

                // -- AUTOMATION 2: ENTERING CLAIMING PHASE --
                bool isEnteringClaiming = CurrentPhase == "Claiming" && setting.CurrentPhase != "Claiming";
                bool ordersTransitioned = false;

                if (isEnteringClaiming)
                {
                    // Fetch every order that is currently SETTLED and ready for pickup
                    var settledOrders = await _context.Orders
                        .Where(o => o.PaymentStatus == "SETTLED")
                        .ToListAsync();

                    if (settledOrders.Any())
                    {
                        foreach (var order in settledOrders)
                        {
                            order.PaymentStatus = "TO CLAIM";
                        }
                        ordersTransitioned = true;
                    }
                }

                // Save phase settings
                setting.CurrentPhase = CurrentPhase;
                setting.PreorderDeadline = PreorderDeadline;

                await _context.SaveChangesAsync();

                // Dynamic Status Messaging
                if (cartsWiped)
                    StatusMessage = "Store phase updated! Pre-orders closed and abandoned carts converted to wishlists.";
                else if (ordersTransitioned)
                    StatusMessage = "Claiming Phase active! All SETTLED orders have automatically been moved to TO CLAIM.";
                else
                    StatusMessage = "Store settings updated successfully.";
            }

            return RedirectToPage();
        }
    }
}