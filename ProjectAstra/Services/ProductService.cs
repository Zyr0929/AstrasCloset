using System.Collections.Generic;
using System.Linq;
using ProjectAstra.Models;

namespace ProjectAstra.Services
{
    public class ProductService
    {
        private readonly List<ApparelProduct> _allProducts;

        public ProductService()
        {
            // mock data array layer maps directly to database content rows.
            _allProducts = new List<ApparelProduct>
            {
                new ApparelProduct(
                    id: 1,
                    name: "TSHIRT 112",
                    price: 320.00m,
                    rating: 4.50,
                    reviewCount: 25,
                    tag: "BACK IN STOCK",
                    description: "Standard School Merch Can Feel A Bit Rigid. We Engineered The Tamaraw Essential Tee To Break That Mold. Built With A Heavy-Weight Cotton Blend And A Relaxed, Slightly Boxy Silhouette, This Tee Gives You That Nonchalant, Streetwear-Ready Drape While Keeping You Completely Comfortable Through Back-To-Back Lectures Or Long Lab Hours.\n\nWhether You're Repping The Clean Aesthetic In Clinical Off-White Or Keeping It Low-Key In Stealth Pitch Black, It's An Absolute Staple For Your Daily Rotation.",
                    variations: new List<ColorVariation>
                    {
                        new ColorVariation("Black", "https://i.ibb.co/zHQpr1Pb/image.png", "https://i.ibb.co/TMm4Vsh3/image.png", "/images/tshirt1_black_extra.png"),
                        new ColorVariation("White", "/images/tshirt1_white_back.png", "/images/tshirt1_white_front.png", "/images/tshirt1_white_extra.png")
                    },
                    availableSizes: new List<string> { "S", "L", "XL", "2XL", "3XL", "5XL" },
                    reviews: new List<CustomerReview>
                    {
                        new CustomerReview { ReviewerName = "Matthew", ReviewDate = "19 Jan,2026", RatingScore = 5, FitStatus = "True to Size", SelectedColor = "Black", SelectedSize = "XL", ReviewBodyText = "Awesome gear. Great purchase." },
                        new CustomerReview { ReviewerName = "Jessica", ReviewDate = "10 Feb,2026", RatingScore = 5, FitStatus = "True to Size", SelectedColor = "Black", SelectedSize = "L", ReviewBodyText = "Great quality, perfectly baggy fabric finish!" },
                        new CustomerReview { ReviewerName = "Jenn", ReviewDate = "18 Feb,2026", RatingScore = 5, FitStatus = "True to Size", SelectedColor = "White", SelectedSize = "M", ReviewBodyText = "My grandson loves it! He was so excited to get these. The quality is great, and passed down to little brother. Thanks!" }
                    },
                    maxLimit: 10
                )
            };
        }

        public List<ApparelProduct> GetAllProducts() => _allProducts;

        public ApparelProduct GetProductById(int id)
        {
            return _allProducts.FirstOrDefault(p => p.Id == id);
        }
    }
}