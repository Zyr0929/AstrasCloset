using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using ProjectAstra.Models; 
using ProjectAstra.Services; 

namespace ProjectAstra.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ProductService _productService; 

        public List<List<ApparelProduct>> ProductPages { get; set; } = new();
        public int TotalPages { get; set; }

        // automatically passes the single shared service here
        public IndexModel(ProductService productService)
        {
            _productService = productService;
        }

        public void OnGet()
        {
            // data from the shared service pointer
            var allProducts = _productService.GetAllProducts();
            int itemsPerPage = 4;

            for (int i = 0; i < allProducts.Count; i += itemsPerPage)
            {
                ProductPages.Add(allProducts.GetRange(i, Math.Min(itemsPerPage, allProducts.Count - i)));
            }

            TotalPages = ProductPages.Count;
            if (TotalPages == 0) TotalPages = 1;
        }
    }
}