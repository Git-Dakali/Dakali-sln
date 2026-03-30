using DK.WebApi.ViewModel.Base;
using DK.WebApi.ViewModel.GeographicLocation.Countries;

namespace DK.WebApi.ViewModel.GeographicLocation.Provinces
{
    public class ProvinceRequest : RequestCode
    {
        public string Name { get; set; }
        public CountryRequest? Country { get; set; }
    }
}
