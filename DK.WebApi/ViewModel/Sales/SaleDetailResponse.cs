using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel.Sales
{
    public class SaleDetailResponse : ResponseGuid
    {
        public ProductResponse? Product { get; set; }
        public VariantResponse? Variant { get; set; }
        public ProductColorResponse? Color { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
        public bool IsExtra { get; set; }
        public StockResponse? Stock { get; set; }
    }
}
