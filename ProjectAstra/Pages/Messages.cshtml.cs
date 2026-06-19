using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectAstra.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectAstra.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class MessagesModel : PageModel
    {
        private readonly AppDbContext _context;
        public MessagesModel(AppDbContext context) => _context = context;

        public List<string> ActiveUsers { get; set; } = new();
        public List<ChatMessage> CurrentChat { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string SelectedUser { get; set; }

        [BindProperty]
        public string AdminReply { get; set; }

        public async Task OnGetAsync()
        {
            // 1. Get a list of all unique users who have sent a message
            ActiveUsers = await _context.ChatMessages
                .Where(m => m.SenderUsername != "Admin")
                .Select(m => m.SenderUsername)
                .Distinct()
                .ToListAsync();

            // 2. Load the chat history if a user is selected
            if (!string.IsNullOrEmpty(SelectedUser))
            {
                CurrentChat = await _context.ChatMessages
                    .Where(m => m.SenderUsername == SelectedUser || m.ReceiverUsername == SelectedUser)
                    .OrderBy(m => m.Timestamp)
                    .ToListAsync();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(AdminReply) || string.IsNullOrEmpty(SelectedUser))
                return RedirectToPage(new { SelectedUser });

            _context.ChatMessages.Add(new ChatMessage
            {
                SenderUsername = "Admin",
                ReceiverUsername = SelectedUser,
                MessageText = AdminReply,
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();
            return RedirectToPage(new { SelectedUser }); // Refreshes the page to show the new message
        }
    }
}