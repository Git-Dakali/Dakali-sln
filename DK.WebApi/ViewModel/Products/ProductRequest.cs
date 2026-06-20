using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel.Products
{
    public class ProductRequest : RequestCode
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public decimal Price { get; set; }
        public decimal SalePrice { get; set; }
        public decimal Weight { get; set; }
        public CategoryRequest? Category { get; set; }
        public IEnumerable<FieldRequest> Fields { get; set; }
        public IEnumerable<VariantRequest> Variants { get; set; }
        public IEnumerable<ProductColorRequest> Colors { get; set; }
        public IEnumerable<ProductSkuRequest> Skus { get; set; }
    }
}
