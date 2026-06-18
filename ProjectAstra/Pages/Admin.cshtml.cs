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

        public int TotalPreOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int PendingApproval { get; set; }
        public int UnderAssessment { get; set; }
        public int VerifiedOrders { get; set; }
        public int RejectedOrders { get; set; }

        public List<Order> RecentOrders { get; set; } = new();

        public async Task OnGetAsync()
        {
            var allOrders = await _context.Orders.OrderByDescending(o => o.OrderDate).ToListAsync();

            TotalPreOrders = allOrders.Count;

            TotalRevenue = allOrders.Where(o => o.PaymentStatus == "VERIFIED").Sum(o => o.TotalPrice);

            PendingApproval = allOrders.Count(o => o.PaymentStatus == "TO PAY" || o.PaymentStatus == "INCOMPLETE");
            UnderAssessment = allOrders.Count(o => o.PaymentStatus == "ASSESSING");
            VerifiedOrders = allOrders.Count(o => o.PaymentStatus == "VERIFIED");
            RejectedOrders = allOrders.Count(o => o.PaymentStatus == "REJECTED");

            RecentOrders = allOrders.Take(10).ToList();
        }
    }
}