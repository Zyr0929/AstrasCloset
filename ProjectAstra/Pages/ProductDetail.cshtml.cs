using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectAstra.Models;
using ProjectAstra.Services;

namespace ProjectAstra.Pages
{
    public class ProductDetailModel : PageModel
    {
        private readonly ProductService _productService;

        public ProductDetailModel(ProductService productService)
        {
            _productService = productService;
        }

        public ApparelProduct Product { get; set; }

        public IActionResult OnGet(int id)
        {
            Product = _productService.GetProductById(id);

            if (Product == null)
            {
                return RedirectToPage("/Store");
            }

            return Page();
        }
    }
}