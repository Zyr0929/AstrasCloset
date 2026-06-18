using System;
using System.Collections.Generic; 
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectAstra.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderReference { get; set; }
        public string Username { get; set; }
        public string CustomerName { get; set; }
        public string YearSection { get; set; }
        public int TotalItems { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime OrderDate { get; set; }

        public List<OrderItem> OrderItems { get; set; } = new();
    }
}