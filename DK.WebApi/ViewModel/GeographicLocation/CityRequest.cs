using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel.GeographicLocation
{
    public class CityRequest : RequestGuid
    {
        public string ZipCode { get; set; }
        public string Name { get; set; }
        public ProvinceRequest? Province { get; set; }
    }
}
