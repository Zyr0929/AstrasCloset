using System.Collections.Generic;

namespace ProjectAstra.Models
{
    public abstract class ProductBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        public string Tag { get; set; }
        public string Description { get; set; } 

        protected ProductBase(int id, string name, decimal price, double rating, int reviewCount, string tag, string description)
        {
            Id = id;
            Name = name;
            Price = price;
            Rating = rating;
            ReviewCount = reviewCount;
            Tag = tag;
            Description = description;
        }
    }

    public class ColorVariation
    {
        public string ColorName { get; set; } 
        public string BackImage { get; set; } 
        public string FrontImage { get; set; }
        public string ExtraImage { get; set; } 

        public ColorVariation(string colorName, string backImage, string frontImage, string extraImage)
        {
            ColorName = colorName;
            BackImage = backImage;
            FrontImage = frontImage;
            ExtraImage = extraImage;
        }
    }

    public class CustomerReview
    {
        public string ReviewerName { get; set; }
        public string ReviewDate { get; set; }
        public int RatingScore { get; set; }
        public string FitStatus { get; set; }
        public string SelectedColor { get; set; }
        public string SelectedSize { get; set; }
        public string ReviewBodyText { get; set; }
    }

    public class ApparelProduct : ProductBase
    {
        public List<ColorVariation> Variations { get; set; }
        public List<string> AvailableSizes { get; set; }
        public List<CustomerReview> Reviews { get; set; }
        public int MaximumPurchaseLimit { get; set; }

        public ApparelProduct(int id, string name, decimal price, double rating, int reviewCount, string tag, string description, List<ColorVariation> variations, List<string> availableSizes, List<CustomerReview> reviews, int maxLimit = 10)
            : base(id, name, price, rating, reviewCount, tag, description)
        {
            Variations = variations;
            AvailableSizes = availableSizes;
            Reviews = reviews;
            MaximumPurchaseLimit = maxLimit;
        }
    }
}