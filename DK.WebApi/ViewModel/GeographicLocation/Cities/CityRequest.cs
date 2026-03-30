using DK.WebApi.ViewModel.Base;
using DK.WebApi.ViewModel.GeographicLocation.Provinces;

namespace DK.WebApi.ViewModel.GeographicLocation.Cities
{
    public class CityRequest : RequestGuid
    {
        public string ZipCode { get; set; }
        public string Name { get; set; }
        public ProvinceRequest? Province { get; set; }
    }
}
