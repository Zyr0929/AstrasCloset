using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectAstra.Models;
using System.ComponentModel.DataAnnotations;

namespace ProjectAstra.Pages;

public class CheckoutModel : PageModel
{
    private readonly AppDbContext _context;
    public CheckoutModel(AppDbContext context)
    {
        _context = context;
    }

    public User CurrentUser { get; set; }
    public List<CartItem> CheckoutItems { get; set; } = new();
    public decimal Subtotal { get; set; }
    public int TotalItems { get; set; }

    [BindProperty]
    public CheckoutForm Form { get; set; } = new();

    public class CheckoutForm
    {
        [Required]
        public string YearSectionStatus { get; set; } = "";
        public string? FacebookLink { get; set; }
        public string? TelegramUsername { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        if (!User.Identity!.IsAuthenticated)
            return RedirectToPage("/Login");

        CurrentUser = await _context.Users
            .FirstOrDefaultAsync(x => x.Username == User.Identity.Name);

        if (CurrentUser == null)
            return RedirectToPage("/Login");

        CheckoutItems = await _context.CartItems
            .Include(x => x.Product)
            .ThenInclude(p => p.Variations)
            .Where(x => x.Username == User.Identity.Name)
            .ToListAsync();

        if (!CheckoutItems.Any())
            return RedirectToPage("/Store");

        TotalItems = CheckoutItems.Sum(x => x.Quantity);

        Subtotal = CheckoutItems.Sum(x =>
            x.Quantity * x.UnitPrice);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!User.Identity!.IsAuthenticated)
            return RedirectToPage("/Login");

        CurrentUser = await _context.Users
            .FirstOrDefaultAsync(x => x.Username == User.Identity.Name);

        if (CurrentUser == null)
            return RedirectToPage("/Login");

        CheckoutItems = await _context.CartItems
            .Include(x => x.Product)
            .Where(x => x.Username == User.Identity.Name)
            .ToListAsync();

        TotalItems = CheckoutItems.Sum(x => x.Quantity);

        Subtotal = CheckoutItems.Sum(x =>
            x.Quantity * x.UnitPrice);

        if (!ModelState.IsValid)
            return Page();

        TempData["Success"] = "Checkout form submitted successfully.";

        return RedirectToPage("/Checkout");
    }
}