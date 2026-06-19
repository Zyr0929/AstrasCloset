using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectAstra.Models;
using Resend;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ProjectAstra.Pages
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly AppDbContext _context;

        public ForgotPasswordModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        [Required(ErrorMessage = "Please enter your Astra Email.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

        [TempData]
        public string StatusMessage { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == Email);

            if (user != null)
            {
                // generate a unique, secure token valid lng for 1 hour
                var token = Guid.NewGuid().ToString("N");
                user.ResetToken = token;
                user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1);
                await _context.SaveChangesAsync();

                var resetLink = Url.Page(
                    "/ResetPassword",
                    pageHandler: null,
                    values: new { email = user.Username, token = token },
                    protocol: Request.Scheme);

                await SendResetEmailAsync(user.Username, user.FirstName, resetLink);
            }

            StatusMessage = "If that email is in our system, we have sent a password reset link to it. Please check your inbox.";
            return RedirectToPage();
        }

        private async Task SendResetEmailAsync(string targetEmail, string firstName, string resetLink)
        {
            IResend resend = ResendClient.Create("re_Thz4XDDz_DBrCyv846BzL6cyTSnf89MeN");

            var htmlEmail = $@"
            <div style='font-family: Arial, sans-serif; background-color: #f8f9fa; padding: 40px 10px;'>
                <div style='max-width: 500px; margin: 0 auto; background-color: #ffffff; border-radius: 8px; border: 1px solid #e0e0e0; overflow: hidden;'>
                    <div style='background-color: #0D4F4E; color: #ffffff; text-align: center; padding: 20px; font-weight: bold; letter-spacing: 2px;'>ASTRA'S Closet</div>
                    <div style='padding: 30px; text-align: center;'>
                        <h2 style='color: #121212;'>Password Reset Request</h2>
                        <p style='color: #666; font-size: 15px; line-height: 1.5;'>Hi {firstName},<br><br>We received a request to reset your password. Click the button below to choose a new one. This link will expire in 1 hour.</p>
                        <a href='{resetLink}' style='display: inline-block; background-color: #D6A019; color: #ffffff; text-decoration: none; padding: 12px 25px; font-weight: bold; border-radius: 4px; margin-top: 15px;'>Reset Password</a>
                        <p style='color: #999; font-size: 12px; margin-top: 25px;'>If you didn't request this, you can safely ignore this email.</p>
                    </div>
                </div>
            </div>";

            var message = new EmailMessage
            {
                From = "onboarding@resend.dev",
                Subject = "Astra's Closet - Password Reset",
                HtmlBody = htmlEmail
            };

            //message.To.Add(targetEmail); same thing as invoice uncomment if my domain na
            message.To.Add("edcyruz54@gmail.com"); 

            await resend.EmailSendAsync(message);
        }
    }
}