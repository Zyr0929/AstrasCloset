using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectAstra.Models;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ProjectAstra.Pages
{
    public class ProfileSettingsModel : PageModel
    {
        private readonly AppDbContext _context;

        public ProfileSettingsModel(AppDbContext context)
        {
            _context = context;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public SocialsInputModel Socials { get; set; } = new();

        public class SocialsInputModel
        {
            [Url(ErrorMessage = "Please enter a valid URL.")]
            public string? FacebookLink { get; set; }
            public string? TelegramUsername { get; set; }
        }

        [BindProperty]
        public PasswordInputModel PasswordSettings { get; set; } = new();

        public class PasswordInputModel
        {
            [Required(ErrorMessage = "Current password is required.")]
            [DataType(DataType.Password)]
            public string CurrentPassword { get; set; } = string.Empty;

            [Required(ErrorMessage = "New password is required.")]
            [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
            [DataType(DataType.Password)]
            public string NewPassword { get; set; } = string.Empty;

            [Required(ErrorMessage = "Please confirm your new password.")]
            [DataType(DataType.Password)]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!User.Identity!.IsAuthenticated) return RedirectToPage("/Login");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == User.Identity.Name);
            if (user == null) return RedirectToPage("/Login");

            Socials = new SocialsInputModel
            {
                FacebookLink = user.FacebookLink,
                TelegramUsername = user.TelegramUsername
            };

            return Page();
        }

        public async Task<IActionResult> OnPostUpdateSocialsAsync()
        {
            if (!User.Identity!.IsAuthenticated) return RedirectToPage("/Login");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == User.Identity.Name);
            if (user == null) return RedirectToPage("/Login");

            user.FacebookLink = Socials.FacebookLink;
            user.TelegramUsername = Socials.TelegramUsername;

            await _context.SaveChangesAsync();

            StatusMessage = "Your social links have been successfully updated!";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostChangePasswordAsync()
        {
            if (!User.Identity!.IsAuthenticated) return RedirectToPage("/Login");
            if (!ModelState.IsValid) return Page();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == User.Identity.Name);
            if (user == null) return RedirectToPage("/Login");

            if (!BCrypt.Net.BCrypt.Verify(PasswordSettings.CurrentPassword, user.PasswordHash))
            {
                ModelState.AddModelError("PasswordSettings.CurrentPassword", "Incorrect current password.");
                return Page();
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(PasswordSettings.NewPassword);

            await _context.SaveChangesAsync();

            StatusMessage = "Your password has been successfully changed!";
            return RedirectToPage();
        }
    }
}