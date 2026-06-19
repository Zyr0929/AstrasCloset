using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectAstra.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectAstra.Pages
{
    public class AdminModel : PageModel
    {
        private readonly AppDbContext _context;

        public AdminModel(AppDbContext context)
        {
            _context = context;
        }

        // --- GLOBAL STORE PHASE ---
        public string CurrentStorePhase { get; set; } = "Preorder";

        // --- PHASE 1 METRICS (Pre-Order) ---
        public int TotalPreOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int PendingApproval { get; set; }

        // --- PHASE 2 METRICS (Validation) ---
        public int UnpaidOrders { get; set; }
        public int SettledOrders { get; set; }
        public int CanceledOrders { get; set; }

        // --- PHASE 3 METRICS (Claiming) ---
        public int OrdersClaimed { get; set; }
        public int OrdersToClaim { get; set; }

        public List<Order> RecentOrders { get; set; } = new();

        // --- PRODUCTION SUMMARY FOR PHASE 2 ---
        public class ProductSummaryDto
        {
            public string NameAndVariation { get; set; }
            public int TotalQuantity { get; set; }
        }
        public List<ProductSummaryDto> ProductSummaries { get; set; } = new();

        public async Task OnGetAsync()
        {
            // 1. Get the current active phase
            var setting = await _context.StoreSettings.FirstOrDefaultAsync();
            if (setting != null)
            {
                CurrentStorePhase = setting.CurrentPhase;
            }

            var allOrders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            // 2. Calculate Metrics using the new Statuses: TO PAY, SETTLED, CANCELED, TO CLAIM, CLAIMED
            TotalPreOrders = allOrders.Count;
            // Revenue only counts orders that are paid
            TotalRevenue = allOrders.Where(o => o.PaymentStatus == "SETTLED" || o.PaymentStatus == "TO CLAIM" || o.PaymentStatus == "CLAIMED").Sum(o => o.TotalPrice);

            // Phase 1
            PendingApproval = allOrders.Count(o => o.PaymentStatus == "TO PAY");

            // Phase 2
            UnpaidOrders = allOrders.Count(o => o.PaymentStatus == "TO PAY");
            SettledOrders = allOrders.Count(o => o.PaymentStatus == "SETTLED" || o.PaymentStatus == "TO CLAIM" || o.PaymentStatus == "CLAIMED");
            CanceledOrders = allOrders.Count(o => o.PaymentStatus == "CANCELED");

            // Phase 3
            OrdersClaimed = allOrders.Count(o => o.PaymentStatus == "CLAIMED");
            OrdersToClaim = allOrders.Count(o => o.PaymentStatus == "SETTLED" || o.PaymentStatus == "TO CLAIM");

            // 3. Generate Production Summary (Only count items from orders that haven't been canceled!)
            var validOrders = allOrders.Where(o => o.PaymentStatus != "CANCELED").ToList();
            var summaryDict = new Dictionary<string, int>();

            foreach (var order in validOrders)
            {
                foreach (var item in order.OrderItems)
                {
                    string key = $"{item.Product.Name} — {item.SelectedColor} / {item.SelectedSize}";
                    if (summaryDict.ContainsKey(key))
                        summaryDict[key] += item.Quantity;
                    else
                        summaryDict[key] = item.Quantity;
                }
            }

            ProductSummaries = summaryDict.Select(kv => new ProductSummaryDto
            {
                NameAndVariation = kv.Key,
                TotalQuantity = kv.Value
            }).OrderByDescending(x => x.TotalQuantity).ToList();

            // Fetch all orders for the table below (DataTables will handle pagination)
            RecentOrders = allOrders.ToList();
        }

        public async Task<IActionResult> OnPostUpdateOrderStatusAsync(int orderId, string newStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                // Safety check: Prevent canceling if it is already settled
                if (newStatus == "CANCELED" && (order.PaymentStatus == "SETTLED" || order.PaymentStatus == "CLAIMED" || order.PaymentStatus == "TO CLAIM"))
                {
                    // Do nothing, you can't cancel a paid order from this button
                    return RedirectToPage();
                }

                order.PaymentStatus = newStatus;
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}