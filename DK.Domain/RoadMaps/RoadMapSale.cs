using Dakali.Domine.Base;
using DK.Domain.Sales;

namespace DK.Domain.RoadMaps
{
    public class RoadMapSale : Entity
    {
        public Sale Sale { get; set; }
        public int SortOrder { get; set; }
    }
}
