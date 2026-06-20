using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel.Products
{
    public class ProductSkuRequest : RequestGuid
    {
        public ProductRequest? Product { get; set; }
        public ProductColorRequest? Color { get; set; }
        public VariantRequest? Variant { get; set; }
        public string Sku { get; set; }
    }
}
