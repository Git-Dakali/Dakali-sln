using DK.WebApi.ViewModel.Base;
using DK.WebApi.ViewModel.Sales;

namespace DK.WebApi.ViewModel.ReturnOrders
{
    public class ReturnOrderResponse : ResponseGuid
    {
        public long Number { get; set; }
        public SaleResponse Sale { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string State { get; set; }
    }
}
