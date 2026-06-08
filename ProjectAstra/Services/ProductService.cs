using System;
using System.Collections.Generic;
using System.Linq;
using ProjectAstra.Models;

namespace ProjectAstra.Services
{
    public class ProductService
    {
        private List<ApparelProduct> _products;

        public ProductService()
        {
            // load data ONCE lang whem app starts up
            _products = InitializeMockData();
        }

        public List<ApparelProduct> GetAllProducts()
        {
            return _products;
        }

        private List<ApparelProduct> InitializeMockData()
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