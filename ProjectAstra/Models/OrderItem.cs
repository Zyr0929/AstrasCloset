using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectAstra.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int ProductId { get; set; }
        public ApparelProduct Product { get; set; }

        public string SelectedColor { get; set; }
        public string SelectedSize { get; set; }
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }
        public bool IsReviewed { get; set; } = false;
    }
}