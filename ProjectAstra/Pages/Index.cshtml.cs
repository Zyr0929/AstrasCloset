using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;

namespace ProjectAstra.Pages
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
        public string ColorName { get; set; } // "black", "white", "teal"
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

    public class IndexModel : PageModel
    {
        public List<List<ApparelProduct>> ProductPages { get; set; } = new();
        public int TotalPages { get; set; }

        public void OnGet()
        {
            var allProducts = GetMockData();
            int itemsPerPage = 4;

            for (int i = 0; i < allProducts.Count; i += itemsPerPage)
            {
                ProductPages.Add(allProducts.GetRange(i, Math.Min(itemsPerPage, allProducts.Count - i)));
            }

            TotalPages = ProductPages.Count;
            if (TotalPages == 0) TotalPages = 1;
        }
        
        //to be updated: static mode, mock data muna, balak with officer dashboard to add their own color, and product itself. 
        private List<ApparelProduct> GetMockData()
        {
            return new List<ApparelProduct>
            {
                new ApparelProduct(1, "TAMARAW TEE v1", 380.00m, 5.0, 24, "BACK IN STOCK", new List<ColorVariation> {
                    new ColorVariation("black", "https://i.ibb.co/v6TLnRZc/image-2026-06-08-012909879.png", "https://i.ibb.co/S4xVtzLY/image.png"),
                    new ColorVariation("white", "https://i.ibb.co/example/white-front.webp", "https://i.ibb.co/example/white-back.webp")
                }),
                new ApparelProduct(2, "FLORENCE TEE", 380.00m, 4.0, 15, "NEW ARRIVAL", new List<ColorVariation> {
                    new ColorVariation("teal", "https://i.ibb.co/v6TLnRZc/image-2026-06-08-012909879.png", "https://i.ibb.co/S4xVtzLY/image.png"),
                    new ColorVariation("white", "https://i.ibb.co/example/white-front.webp", "https://i.ibb.co/example/white-back.webp")
                }),
                new ApparelProduct(3, "NURSING STARTER PACK", 380.00m, 5.0, 42, "BACK IN STOCK", new List<ColorVariation> {
                    new ColorVariation("black", "https://i.ibb.co/v6TLnRZc/image-2026-06-08-012909879.png", "https://i.ibb.co/S4xVtzLY/image.png")
                }),
                new ApparelProduct(4, "ASTRA BASIC ESSENTIALS", 380.00m, 3.5, 19, "BACK IN STOCK", new List<ColorVariation> {
                    new ColorVariation("white", "https://i.ibb.co/v6TLnRZc/image-2026-06-08-012909879.png", "https://i.ibb.co/S4xVtzLY/image.png")
                }),
                new ApparelProduct(5, "TAMARAW TEE v2", 380.00m, 4.5, 31, "NEW ARRIVAL", new List<ColorVariation> {
                    new ColorVariation("teal", "https://i.ibb.co/v6TLnRZc/image-2026-06-08-012909879.png", "https://i.ibb.co/S4xVtzLY/image.png"),
                    new ColorVariation("black", "https://i.ibb.co/v6TLnRZc/image-2026-06-08-012909879.png", "https://i.ibb.co/example/t2-black-back.webp")
                }),
                new ApparelProduct(6, "CRITICAL CARE HOODIE", 380.00m, 5.0, 8, "NEW ARRIVAL", new List<ColorVariation> {
                    new ColorVariation("black", "https://i.ibb.co/v6TLnRZc/image-2026-06-08-012909879.png", "https://i.ibb.co/S4xVtzLY/image.png")
                }),
                new ApparelProduct(7, "CLINICAL OVERSIZED TEE", 380.00m, 4.0, 11, "BACK IN STOCK", new List<ColorVariation> {
                    new ColorVariation("white", "https://i.ibb.co/v6TLnRZc/image-2026-06-08-012909879.png", "https://i.ibb.co/S4xVtzLY/image.png")
                }),
                new ApparelProduct(8, "ROUNDS COMFORT SHIRT", 380.00m, 4.5, 22, "BACK IN STOCK", new List<ColorVariation> {
                    new ColorVariation("black", "https://i.ibb.co/v6TLnRZc/image-2026-06-08-012909879.png", "https://i.ibb.co/S4xVtzLY/image.png"),
                    new ColorVariation("teal", "https://i.ibb.co/v6TLnRZc/image-2026-06-08-012909879.png", "https://i.ibb.co/example/rounds-teal-back.webp")
                })
            };
        }
    }
}