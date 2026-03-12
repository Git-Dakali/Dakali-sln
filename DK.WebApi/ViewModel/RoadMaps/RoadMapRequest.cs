using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel.RoadMaps
{
    public class RoadMapRequest : RequestGuid
    {
        public long Number { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? TravelDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public DriverRequest Driver { get; set; }
        public string State { get; set; }
        public IEnumerable<RoadMapSaleRequest> Sales { get; set; }
    }
}
