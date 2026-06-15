using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ProjectAstra.Models; // Added to access your ApparelProduct model

namespace ProjectAstra.Pages
{
    public class VariationUploadData
    {
        public string ColorName { get; set; }
        public IFormFile FrontImage { get; set; }
        public IFormFile BackImage { get; set; }
        public IFormFile ExtraImage { get; set; }
    }

    public class AddProductModel : PageModel
    {
        private readonly IWebHostEnvironment _env;
        private readonly AppDbContext _context; // 1. Added your database context!

        public AddProductModel(IWebHostEnvironment env, AppDbContext context)
        {
            _env = env;
            _context = context;
        }

        [BindProperty]
        public List<VariationUploadData> Variations { get; set; } = new List<VariationUploadData>();

        [BindProperty] public string ProductName { get; set; }
        [BindProperty] public decimal Price { get; set; }
        [BindProperty] public string Theme { get; set; }
        [BindProperty] public string Tags { get; set; }
        [BindProperty] public string Description { get; set; }
        [BindProperty] public string Category { get; set; }
        [BindProperty] public string Sizes { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            string folderPath = Path.Combine(_env.WebRootPath, "images", "products");
            Directory.CreateDirectory(folderPath);

            // 2. Prepare the new product object
            var newProduct = new ApparelProduct
            {
                Name = ProductName,
                Price = Price,
                Description = Description,
                Tag = string.IsNullOrEmpty(Tags) ? Category : Tags, // Fallback to Category if Tags is empty
                AvailableSizes = Sizes != null ? Sizes.Split(',').ToList() : new List<string>(),
                Rating = 0,
                ReviewCount = 0,
                MaximumPurchaseLimit = 5, // Default pre-order limit
                Variations = new List<ColorVariation>()
            };

            foreach (var variation in Variations)
            {
                if (variation.FrontImage != null && variation.BackImage != null && variation.ExtraImage != null)
                {
                    // Generate unique filenames
                    string frontFileName = Guid.NewGuid().ToString() + Path.GetExtension(variation.FrontImage.FileName);
                    string backFileName = Guid.NewGuid().ToString() + Path.GetExtension(variation.BackImage.FileName);
                    string extraFileName = Guid.NewGuid().ToString() + Path.GetExtension(variation.ExtraImage.FileName);

                    // Save images physically
                    using (var stream = new FileStream(Path.Combine(folderPath, frontFileName), FileMode.Create))
                        await variation.FrontImage.CopyToAsync(stream);

                    using (var stream = new FileStream(Path.Combine(folderPath, backFileName), FileMode.Create))
                        await variation.BackImage.CopyToAsync(stream);

                    using (var stream = new FileStream(Path.Combine(folderPath, extraFileName), FileMode.Create))
                        await variation.ExtraImage.CopyToAsync(stream);

                    // 3. Attach the saved image paths to the new product
                    newProduct.Variations.Add(new ColorVariation
                    {
                        ColorName = variation.ColorName,
                        FrontImage = "/images/products/" + frontFileName,
                        BackImage = "/images/products/" + backFileName,
                        ExtraImage = "/images/products/" + extraFileName
                    });
                }
            }

            // 4. Officially save it to the SQL Database!
            _context.Products.Add(newProduct);
            await _context.SaveChangesAsync();

            // Redirect back to the store so you can immediately see it!
            return RedirectToPage("/Store");
        }
    }
}