using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectAstra.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace ProjectAstra.Pages
{
    public class LoginModel : PageModel
    {
        private readonly AppDbContext _context;

        public LoginModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
        public string? UserFirstName { get; set; }
        public string RedirectUrl { get; set; } = "/Index";
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == Username);

            if (user != null && BCrypt.Net.BCrypt.Verify(Password, user.PasswordHash))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim("FirstName", user.FirstName),
                    new Claim(ClaimTypes.Email, user.Username),
                    new Claim(ClaimTypes.Role, user.Role), 
                    new Claim("AstraId", user.AstraId)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                UserFirstName = user.FirstName;
                SuccessMessage = "Login successful! Redirecting...";

                if (user.Role == "Admin")
                {
                    RedirectUrl = "/Admin";
                }
                else
                {
                    RedirectUrl = "/Index";
                }

                return Page();
            }
            else
            {
                ErrorMessage = "Invalid username or password.";
                return Page();
            }
        }
    }
}