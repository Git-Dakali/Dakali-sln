using DK.WebApi.ViewModel.Base;
using DK.WebApi.ViewModel.Products;

namespace DK.WebApi.ViewModel.Sales
{
    public class SaleDetailRequest : RequestGuid
    {
        public ProductRequest? Product { get; set; }
        public ProductSkuRequest? ProductSku { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
        public bool IsExchangeItem { get; set; }
        public StockRequest? Stock { get; set; }
    }
}
