using DK.WebApi.ViewModel.Base;
using DK.WebApi.ViewModel.GeographicLocation.Cities;

namespace DK.WebApi.ViewModel.Sales
{
    public class SaleRequest : RequestGuid
    {
        public string Identifier { get; set; }
        public string Dni { get; set; }
        public string Cuit { get; set; }
        public long Number { get; set; }
        public string ArcaNumber { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public TimeSpan DeliveryStartTime { get; set; }
        public TimeSpan DeliveryEndTime { get; set; }
        public string BusinessName { get; set; }
        public string Address { get; set; }
        public string Floor { get; set; }
        public string Apartment { get; set; }
        public string Phone { get; set; }
        public string Observation { get; set; }
        public decimal GrossPrice { get; set; }
        public decimal Discounts { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal ShippingPrice { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public TaxStatusRequest? TaxStatus { get; set; }
        public OriginSaleRequest? OriginSale { get; set; }
        public StoredFileRequest? PdfInvoice { get; set; }
        public CityRequest? City { get; set; }
        public string State { get; set; }
        public IEnumerable<SaleDetailRequest> SaleDetails { get; set; }
    }
}
