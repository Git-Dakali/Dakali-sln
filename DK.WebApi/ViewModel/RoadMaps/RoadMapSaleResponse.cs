using DK.WebApi.ViewModel.Base;
using DK.WebApi.ViewModel.Sales;

namespace DK.WebApi.ViewModel.RoadMaps
{
    public class RoadMapSaleResponse : Response
    {
        public SaleResponse Sale { get; set; }
        public int SortOrder { get; set; }
    }
}
