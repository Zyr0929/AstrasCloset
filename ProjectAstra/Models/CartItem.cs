using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 

namespace ProjectAstra.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        public int ProductId { get; set; }
        public ApparelProduct Product { get; set; }

        [Required]
        public string SelectedColor { get; set; }

        [Required]
        public string SelectedSize { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")] 
        public decimal UnitPrice { get; set; }
    }
}