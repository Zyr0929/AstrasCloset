using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectAstra.Models
{
    public class WishlistItem
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; } 

        public int ProductId { get; set; }
        public ApparelProduct Product { get; set; } 

        public DateTime DateAdded { get; set; } = DateTime.Now;
    }
}