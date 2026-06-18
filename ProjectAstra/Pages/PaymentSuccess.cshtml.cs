using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectAstra.Models;

namespace ProjectAstra.Pages;

public class PaymentSuccessModel : PageModel
{
    private readonly AppDbContext _context;

    public PaymentSuccessModel(AppDbContext context)
    {
        _context = context;
    }

    public string OrderId { get; set; } = "";

    public async Task<IActionResult> OnGetAsync(string orderId)
    {
        OrderId = orderId;

        var order = await _context.Orders
            .FirstOrDefaultAsync(x =>
                x.OrderReference == orderId);

        if (order != null)
        {
            order.PaymentStatus = "PAID";

            await _context.SaveChangesAsync();
        }

        return Page();
    }
}