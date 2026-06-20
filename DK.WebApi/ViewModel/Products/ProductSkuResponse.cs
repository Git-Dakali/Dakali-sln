using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel.Products
{
    public class ProductSkuResponse : ResponseGuid
    {
        public ProductResponse? Product { get; set; }
        public ProductColorResponse? Color { get; set; }
        public VariantResponse? Variant { get; set; }
        public string Sku { get; set; }
    }
}
