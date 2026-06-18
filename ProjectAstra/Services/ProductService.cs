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
                
            };
        }

        public List<ApparelProduct> GetAllProducts() => _allProducts;

        public ApparelProduct GetProductById(int id)
        {
            return _allProducts.FirstOrDefault(p => p.Id == id);
        }
    }
}