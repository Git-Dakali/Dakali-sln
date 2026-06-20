using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel.Products
{
    public class ProductResponse : ResponseCode
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public decimal Price { get; set; }
        public decimal SalePrice { get; set; }
        public decimal Weight { get; set; }
        public CategoryResponse? Category { get; set; }
        public IEnumerable<FieldResponse> Fields { get; set; }
        public IEnumerable<VariantResponse> Variants { get; set; }
        public IEnumerable<ProductColorResponse> Colors { get; set; }
        public IEnumerable<ProductSkuResponse> Skus { get; set; }
    }
}
