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

    public class ApparelProduct : ProductBase
    {
        public List<string> Colors { get; set; }
        public string FrontImage { get; set; }
        public string BackImage { get; set; }

        public ApparelProduct(int id, string name, decimal price, double rating, int reviewCount, string tag, string frontImg, string backImg, List<string> colors)
            : base(id, name, price, rating, reviewCount, tag)
        {
            FrontImage = frontImg;
            BackImage = backImg;
            Colors = colors;
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

        private List<ApparelProduct> GetMockData()
        {
            return new List<ApparelProduct>
            {
                new ApparelProduct(1, "TAMARAW TEE v1", 380.00m, 5.0, 24, "BACK IN STOCK", "https://i.ibb.co/hJ2B1PRS/test-INDEX.webp", "https://i.ibb.co/hJ2B1PRS/test-INDEX.webp", new List<string> { "black", "white" }),
                new ApparelProduct(2, "FLORENCE TEE", 380.00m, 4.0, 15, "NEW ARRIVAL", "https://i.ibb.co/hJ2B1PRS/test-INDEX.webp", "https://i.ibb.co/hJ2B1PRS/test-INDEX.webp", new List<string> { "teal", "white" }),
                new ApparelProduct(3, "NURSING STARTER PACK", 380.00m, 5.0, 42, "BACK IN STOCK", "https://i.ibb.co/hJ2B1PRS/test-INDEX.webp", "https://i.ibb.co/hJ2B1PRS/test-INDEX.webp", new List<string> { "black" }),
                new ApparelProduct(4, "ASTRA BASIC ESSENTIALS", 380.00m, 3.5, 19, "BACK IN STOCK", "https://i.ibb.co/hJ2B1PRS/test-INDEX.webp", "https://i.ibb.co/hJ2B1PRS/test-INDEX.webp", new List<string> { "white" }),
                new ApparelProduct(5, "TAMARAW TEE v2", 380.00m, 4.5, 31, "NEW ARRIVAL", "https://i.ibb.co/hJ2B1PRS/test-INDEX.webp", "https://i.ibb.co/hJ2B1PRS/test-INDEX.webp", new List<string> { "teal", "black" }),
                new ApparelProduct(6, "CRITICAL CARE HOODIE", 380.00m, 5.0, 8, "NEW ARRIVAL", "https://i.ibb.co/hJ2B1PRS/test-INDEX.webp", "https://i.ibb.co/hJ2B1PRS/test-INDEX.webp", new List<string> { "black" }),
                new ApparelProduct(7, "CLINICAL OVERSIZED TEE", 380.00m, 4.0, 11, "BACK IN STOCK", "https://i.ibb.co/hJ2B1PRS/test-INDEX.webp", "https://i.ibb.co/hJ2B1PRS/test-INDEX.webp", new List<string> { "white" }),
                new ApparelProduct(8, "ROUNDS COMFORT SHIRT", 380.00m, 4.5, 22, "BACK IN STOCK", "https://i.ibb.co/hJ2B1PRS/test-INDEX.webp", "https://i.ibb.co/hJ2B1PRS/test-INDEX.webp", new List<string> { "black", "teal" })
            };
        }
    }
}