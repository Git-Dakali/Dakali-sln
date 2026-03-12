using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel.RoadMaps
{
    public class DriverRequest : RequestGuid
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Dni { get; set; }
    }
}
