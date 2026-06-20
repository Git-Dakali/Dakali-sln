using DK.WebApi.ViewModel.Base;
using DK.WebApi.ViewModel.Sales;

namespace DK.WebApi.ViewModel.ReturnOrders
{
    public class ReturnOrderRequest : RequestGuid
    {
        public long Number { get; set; }
        public SaleRequest Sale { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string State { get; set; }
    }
}
