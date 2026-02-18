using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel.GeographicLocation
{
    public class CityResponse : ResponseGuid
    {
        public string ZipCode { get; set; }
        public string Name { get; set; }
        public ProvinceResponse? Province { get; set; }
    }
}
