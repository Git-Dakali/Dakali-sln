using DK.Domain.GeographicLocation;
using DK.WebApi.ViewModel.GeographicLocation.Cities;

namespace DK.WebApi.ViewModel.Sales
{
    public class SaleLocationRequest
    {
        public long SaleId { get; set; }
        public string Address { get; set; }
        public string Observation { get; set; }
        public CityRequest? City { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
    }
}
