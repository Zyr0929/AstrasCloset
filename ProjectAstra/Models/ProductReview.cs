using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectAstra.Models
{
    public class ProductReview
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public ApparelProduct Product { get; set; }

        public string Username { get; set; }

        public string ReviewerFirstName { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(500)]
        public string ReviewText { get; set; }

        public DateTime ReviewDate { get; set; }

        public bool IsApproved { get; set; } = true;
    }
}