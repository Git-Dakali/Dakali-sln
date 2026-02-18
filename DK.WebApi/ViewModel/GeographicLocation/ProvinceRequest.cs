using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel.GeographicLocation
{
    public class ProvinceRequest : RequestCode
    {
        public string Name { get; set; }
        public CountryRequest? Country { get; set; }
    }
}
