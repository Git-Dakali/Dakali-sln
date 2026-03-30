using DK.WebApi.ViewModel.Base;
using DK.WebApi.ViewModel.GeographicLocation.Countries;

namespace DK.WebApi.ViewModel.GeographicLocation.Provinces
{
    public class ProvinceResponse : ResponseCode
    {
        public string Name { get; set; }
        public CountryResponse? Country { get; set; }
    }
}
