using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectAstra.Models
{
    public abstract class ProductBase
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Category { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        public string Tag { get; set; }
        public string Description { get; set; }
        public bool IsArchived { get; set; } = false;

        protected ProductBase() { }

        protected ProductBase(int id, string name, string category, decimal price, double rating, int reviewCount, string tag, string description)
        {
            Id = id;
            Name = name;
            Category = category;
            Price = price;
            Rating = rating;
            ReviewCount = reviewCount;
            Tag = tag;
            Description = description;
        }
    }

    public class ColorVariation
    {
        public int Id { get; set; }
        public string ColorName { get; set; }
        public string BackImage { get; set; }
        public string FrontImage { get; set; }
        public string ExtraImage { get; set; }

        public ColorVariation() { }

        public ColorVariation(string colorName, string backImage, string frontImage, string extraImage)
        {
            ColorName = colorName; BackImage = backImage; FrontImage = frontImage; ExtraImage = extraImage;
        }
    }

    public class CustomerReview
    {
        public int Id { get; set; }
        [Required] public string ReviewerName { get; set; } = string.Empty;
        [Required] public string ReviewDate { get; set; } = string.Empty;
        [Required] public int RatingScore { get; set; }
        [Required] public string FitStatus { get; set; } = string.Empty;
        [Required] public string SelectedColor { get; set; } = string.Empty;
        [Required] public string SelectedSize { get; set; } = string.Empty;
        [Required] public string ReviewBodyText { get; set; } = string.Empty;
    }


    public class ApparelProduct : ProductBase
    {
        public List<string> Types { get; set; } = new();
        public List<ColorVariation> Variations { get; set; } = new();
        public List<string> AvailableSizes { get; set; } = new();
        public List<ProductReview> Reviews { get; set; } = new();
        public int MaximumPurchaseLimit { get; set; }

        public ApparelProduct() { }

        public ApparelProduct(int id, string name, string category, decimal price, double rating, int reviewCount, string tag, string description, List<ColorVariation> variations, List<string> availableSizes, List<CustomerReview> reviews, int maxLimit = 10)
            : base(id, name, category, price, rating, reviewCount, tag, description)
        {
            Variations = variations; AvailableSizes = availableSizes; Reviews = Reviews; MaximumPurchaseLimit = maxLimit;
        }
    }

}