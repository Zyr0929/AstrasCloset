using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectAstra.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ProjectAstra.Pages
{
    public class ResetPasswordModel : PageModel
    {
        private readonly AppDbContext _context;

        public ResetPasswordModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required]
            public string Email { get; set; } = string.Empty;

            [Required]
            public string Token { get; set; } = string.Empty;

            [Required(ErrorMessage = "Please enter a new password.")]
            [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
            [DataType(DataType.Password)]
            public string NewPassword { get; set; } = string.Empty;

            [Required(ErrorMessage = "Please confirm your new password.")]
            [DataType(DataType.Password)]
            [Compare("NewPassword", ErrorMessage = "The passwords do not match.")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public IActionResult OnGet(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                StatusMessage = "Error: Invalid password reset link.";
                return RedirectToPage("/Login");
            }

            // Load the hidden fields so they get submitted in the POST request
            Input.Email = email;
            Input.Token = token;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == Input.Email && u.ResetToken == Input.Token);

            // Verify if token exists and hasn't expired
            if (user == null || user.ResetTokenExpiry == null || user.ResetTokenExpiry < DateTime.UtcNow)
            {
                ModelState.AddModelError(string.Empty, "This password reset link is invalid or has expired. Please request a new one.");
                return Page();
            }

            // Hash the new password and update the database
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(Input.NewPassword);

            // Clear the tokens so the link cannot be clicked again!
            user.ResetToken = null;
            user.ResetTokenExpiry = null;

            await _context.SaveChangesAsync();

            StatusMessage = "Your password has been successfully reset! You can now log in.";
            return RedirectToPage("/Login");
        }
    }
}