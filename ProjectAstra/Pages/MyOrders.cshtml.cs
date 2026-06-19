using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectAstra.Models;

namespace ProjectAstra.Pages;

public class MyOrdersModel : PageModel
{
    private readonly AppDbContext _context;

    public MyOrdersModel(AppDbContext context)
    {
        _context = context;
    }

    public List<Order> MyOrders { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        if (!User.Identity!.IsAuthenticated)
            return RedirectToPage("/Login");

        var username = User.Identity.Name;

        MyOrders = await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .Where(o => o.Username == username)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        return Page();
    }
}