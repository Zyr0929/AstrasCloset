using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectAstra.Models;
using ProjectAstra.Services;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace ProjectAstra.Pages
{
    public class CheckoutModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly XenditService _xendit;

        public CheckoutModel(AppDbContext context, XenditService xendit)
        {
            _context = context;
            _xendit = xendit;
        }

        public User CurrentUser { get; set; }
        public List<CartItem> CheckoutItems { get; set; } = new();
        public decimal Subtotal { get; set; }
        public int TotalItems { get; set; }

        [BindProperty]
        public CheckoutForm Form { get; set; } = new();

        public class CheckoutForm
        {
            [Required(ErrorMessage = "Please select your year and section to proceed.")]
            public string YearSectionStatus { get; set; } = "";
            public string? FacebookLink { get; set; }
            public string? TelegramUsername { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!User.Identity!.IsAuthenticated) return RedirectToPage("/Login");

            CurrentUser = await _context.Users.FirstOrDefaultAsync(x => x.Username == User.Identity.Name);
            if (CurrentUser == null) return RedirectToPage("/Login");

            CheckoutItems = await _context.CartItems
                .Include(x => x.Product)
                .ThenInclude(p => p.Variations)
                .Where(x => x.Username == User.Identity.Name)
                .ToListAsync();

            if (!CheckoutItems.Any()) return RedirectToPage("/Store");

            TotalItems = CheckoutItems.Sum(x => x.Quantity);
            Subtotal = CheckoutItems.Sum(x => x.Quantity * x.UnitPrice);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.Identity!.IsAuthenticated) return RedirectToPage("/Login");

            CurrentUser = await _context.Users.FirstOrDefaultAsync(x => x.Username == User.Identity.Name);
            if (CurrentUser == null) return RedirectToPage("/Login");

            CheckoutItems = await _context.CartItems
                .Include(x => x.Product)
                .Where(x => x.Username == User.Identity.Name)
                .ToListAsync();

            TotalItems = CheckoutItems.Sum(x => x.Quantity);
            Subtotal = CheckoutItems.Sum(x => x.Quantity * x.UnitPrice);

            if (!ModelState.IsValid) return Page();

            var orderRef = $"ORD-{DateTime.Now:yyyyMMddHHmmss}";
            var order = new Order
            {
                OrderReference = orderRef,
                Username = CurrentUser.Username,
                CustomerName = $"{CurrentUser.FirstName} {CurrentUser.LastName}",
                YearSection = Form.YearSectionStatus,
                TotalItems = TotalItems,
                TotalPrice = Subtotal,
                PaymentStatus = "TO PAY",
                OrderDate = DateTime.Now,
                OrderItems = new List<OrderItem>() 
            };

            foreach (var cartItem in CheckoutItems)
            {
                order.OrderItems.Add(new OrderItem
                {
                    ProductId = cartItem.ProductId,
                    SelectedColor = cartItem.SelectedColor,
                    SelectedSize = cartItem.SelectedSize,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.UnitPrice
                });
            }

            _context.Orders.Add(order);

            _context.CartItems.RemoveRange(CheckoutItems);

            await _context.SaveChangesAsync();

            var paymentUrl = await _xendit.CreateInvoice(orderRef, Subtotal, CurrentUser.Username);

            return Redirect(paymentUrl);
        }
    }
}