using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectAstra.Models
{
    public class Order
    {
        public int Id { get; set; }

        public string OrderReference { get; set; } // e.g., "ORD-2026-0101"
        public string Username { get; set; } // The logged-in user who made it
        public string CustomerName { get; set; }
        public string YearSection { get; set; }

        public int TotalItems { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        // Statuses: "TO PAY", "INCOMPLETE", "ASSESSING", "VERIFIED", "REJECTED"
        public string PaymentStatus { get; set; }

        public DateTime OrderDate { get; set; }
    }
}