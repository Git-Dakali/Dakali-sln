using DK.WebApi.ViewModel.Base;
using DK.WebApi.ViewModel.Products;

namespace DK.WebApi.ViewModel.Sales
{
    public class SaleDetailResponse : ResponseGuid
    {
        public ProductResponse? Product { get; set; }
        public ProductSkuResponse? ProductSku { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
        public bool IsExchangeItem { get; set; }
        public StockResponse? Stock { get; set; }
    }
}
