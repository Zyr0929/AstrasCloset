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

        protected ProductBase(int id, string name, decimal price, double rating, int reviewCount, string tag)
        {
            Id = id;
            Name = name;
            Price = price;
            Rating = rating;
            ReviewCount = reviewCount;
            Tag = tag;
        }
    }

    public class ColorVariation
    {
        public string ColorName { get; set; } // "black", "white", "teal"  tas iba pa
        public string FrontImage { get; set; }
        public string BackImage { get; set; }

        public ColorVariation(string colorName, string frontImage, string backImage)
        {
            ColorName = colorName;
            FrontImage = frontImage;
            BackImage = backImage;
        }
    }

    public class ApparelProduct : ProductBase
    {
        public List<ColorVariation> Variations { get; set; }

        public ApparelProduct(int id, string name, decimal price, double rating, int reviewCount, string tag, List<ColorVariation> variations)
            : base(id, name, price, rating, reviewCount, tag)
        {
            Variations = variations;
        }
    }
}