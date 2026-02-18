using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel.Sales
{
    public class SaleDetailRequest : RequestGuid
    {
        public ProductRequest? Product { get; set; }
        public VariantRequest? Variant { get; set; }
        public ProductColorRequest? Color { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
        public bool IsExtra { get; set; }
        public StockRequest? Stock { get; set; }
    }
}
