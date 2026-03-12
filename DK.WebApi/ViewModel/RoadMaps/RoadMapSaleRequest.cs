using DK.WebApi.ViewModel.Base;
using DK.WebApi.ViewModel.Sales;

namespace DK.WebApi.ViewModel.RoadMaps
{
    public class RoadMapSaleRequest : Request
    {
        public SaleRequest Sale { get; set; }
        public int SortOrder { get; set; }
    }
}
