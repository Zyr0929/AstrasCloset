using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectAstra.Models;
using BCrypt.Net;

namespace ProjectAstra.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly AppDbContext _context;

        public RegisterModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string FirstName { get; set; } = string.Empty;

        [BindProperty]
        public string LastName { get; set; } = string.Empty;

        [BindProperty]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        public string ContactNumber { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (!Username.EndsWith("@astra.feu-nrmf.edu.ph", StringComparison.OrdinalIgnoreCase))
            {
                ErrorMessage = "Access Denied: Only @astra.feu-nrmf.edu.ph email addresses are allowed.";
                return Page();
            }

            var textInfo = System.Globalization.CultureInfo.CurrentCulture.TextInfo;
            FirstName = textInfo.ToTitleCase(FirstName.ToLower());
            LastName = textInfo.ToTitleCase(LastName.ToLower());

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == Username);
            if (existingUser != null)
            {
                ErrorMessage = "An account with this email already exists.";
                return Page();
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(Password);

            var newUser = new User
            {
                AstraId = GenerateAstraId(), 
                FirstName = FirstName,
                LastName = LastName,
                Username = Username,
                ContactNumber = ContactNumber,
                PasswordHash = hashedPassword
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            SuccessMessage = "Registration successful! Redirecting to login...";
            return Page();
        }

        // Unique ID per customer tired na ako
        private string GenerateAstraId()
        {
            var year = DateTime.Now.Year.ToString(); 
            var random = new Random();

            var part1 = random.Next(0, 10000).ToString("D4");
            var part2 = random.Next(0, 10000).ToString("D4");
            var part3 = random.Next(0, 10000).ToString("D4");

            return $"{year}-{part1}-{part2}-{part3}";
        }
    }
}