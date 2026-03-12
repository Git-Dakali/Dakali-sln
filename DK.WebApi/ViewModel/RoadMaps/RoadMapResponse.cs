using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel.RoadMaps
{
    public class RoadMapResponse : ResponseGuid
    {
        public long Number { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? TravelDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public DriverResponse Driver { get; set; }
        public string State { get; set; }
        public IEnumerable<RoadMapSaleResponse> Sales { get; set; }
    }
}
