using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectAstra.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectAstra.Pages
{
    [Authorize]
    public class WriteReviewModel : PageModel
    {
        private readonly AppDbContext _context;
        public WriteReviewModel(AppDbContext context) => _context = context;

        public OrderItem TargetItem { get; set; }

        [BindProperty]
        public ProductReview NewReview { get; set; } = new();

        [BindProperty]
        public int OrderItemId { get; set; }

        public async Task<IActionResult> OnGetAsync(int itemId)
        {
            TargetItem = await _context.OrderItems
                .Include(oi => oi.Order)
                .Include(oi => oi.Product)
                .FirstOrDefaultAsync(oi => oi.Id == itemId);

            // Verify they actually own the item and it's CLAIMED
            if (TargetItem == null ||
                TargetItem.Order.Username != User.Identity.Name ||
                TargetItem.Order.PaymentStatus != "CLAIMED" ||
                TargetItem.IsReviewed)
            {
                return RedirectToPage("/MyOrders");
            }

            OrderItemId = TargetItem.Id;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var item = await _context.OrderItems
                .Include(oi => oi.Order)
                .Include(oi => oi.Product)
                .ThenInclude(p => p.Reviews)
                .FirstOrDefaultAsync(oi => oi.Id == OrderItemId);

            if (item == null || item.IsReviewed) return RedirectToPage("/MyOrders");

            // Attach required background data
            NewReview.ProductId = item.ProductId;
            NewReview.Username = User.Identity.Name;
            NewReview.ReviewerFirstName = User.FindFirst("FirstName")?.Value ?? "Student";
            NewReview.ReviewDate = DateTime.Now;
            NewReview.IsApproved = true;

            // Save review & mark item as reviewed
            _context.Set<ProductReview>().Add(NewReview);
            item.IsReviewed = true;

            // Recalculate global product star rating
            var product = item.Product;
            int totalReviews = product.Reviews.Count + 1;
            double totalScore = product.Reviews.Sum(r => r.Rating) + NewReview.Rating;

            product.ReviewCount = totalReviews;
            product.Rating = totalScore / totalReviews;

            await _context.SaveChangesAsync();
            return RedirectToPage("/MyOrders");
        }
    }
}