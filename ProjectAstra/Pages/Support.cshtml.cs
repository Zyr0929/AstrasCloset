using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectAstra.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectAstra.Pages
{
    [Authorize]
    public class SupportModel : PageModel
    {
        private readonly AppDbContext _context;
        public SupportModel(AppDbContext context) => _context = context;

        public List<ChatMessage> Messages { get; set; } = new();

        [BindProperty]
        public string NewMessage { get; set; }

        public async Task OnGetAsync()
        {
            var currentUser = User.Identity!.Name;
            Messages = await _context.ChatMessages
                .Where(m => m.SenderUsername == currentUser || m.ReceiverUsername == currentUser)
                .OrderBy(m => m.Timestamp)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(NewMessage)) return RedirectToPage();

            var currentUser = User.Identity!.Name;

            _context.ChatMessages.Add(new ChatMessage
            {
                SenderUsername = currentUser,
                ReceiverUsername = "Admin",
                MessageText = NewMessage,
                Timestamp = DateTime.Now
            });

            var pastAdminReplies = await _context.ChatMessages
                .Where(m => m.ReceiverUsername == currentUser && m.SenderUsername == "Admin")
                .ToListAsync();

            bool needsAutoReply = !pastAdminReplies.Any() ||
                                  pastAdminReplies.Max(m => m.Timestamp) < DateTime.Now.AddHours(-12);

            if (needsAutoReply)
            {
                _context.ChatMessages.Add(new ChatMessage
                {
                    SenderUsername = "Admin",
                    ReceiverUsername = currentUser,
                    MessageText = "Hello! Thank you for reaching out to Astra's Closet. We have received your message. Please note that our Student Council officers are currently in class, but we will review your concern and reply as soon as possible. Thank you for your patience!",
                    Timestamp = DateTime.Now.AddSeconds(1) 
                });
            }

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }
    }
}