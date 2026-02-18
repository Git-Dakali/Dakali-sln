using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel.GeographicLocation
{
    public class ProvinceResponse : ResponseCode
    {
        public string Name { get; set; }
        public CountryResponse? Country { get; set; }
    }
}
