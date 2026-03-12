using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel.RoadMaps
{
    public class DriverResponse : ResponseGuid
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Dni { get; set; }
    }
}
