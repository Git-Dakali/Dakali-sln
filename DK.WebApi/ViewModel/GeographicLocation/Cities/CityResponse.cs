using DK.WebApi.ViewModel.Base;
using DK.WebApi.ViewModel.GeographicLocation.Provinces;

namespace DK.WebApi.ViewModel.GeographicLocation.Cities
{
    public class CityResponse : ResponseGuid
    {
        public string ZipCode { get; set; }
        public string Name { get; set; }
        public ProvinceResponse? Province { get; set; }
    }
}
