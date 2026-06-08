using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using ProjectAstra.Models;
using ProjectAstra.Services;

namespace ProjectAstra.Pages
{
    public class StoreModel : PageModel
    {
        private readonly ProductService _productService;

        public List<ApparelProduct> StoreProducts { get; set; } = new();

        public StoreModel(ProductService productService)
        {
            _productService = productService;
        }

        public void OnGet()
        {
            StoreProducts = _productService.GetAllProducts();
        }   
    }
}
