using DK.WebApi.ViewModel.Base;
using DK.WebApi.ViewModel.GeographicLocation.Cities;

namespace DK.WebApi.ViewModel.Sales
{
    public class SaleResponse : ResponseGuid
    {
        public bool IsPrinted { get; set; }
        public bool IsReverseLogistics { get; set; }
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
        public TaxStatusResponse? TaxStatus { get; set; }
        public OriginSaleResponse? OriginSale { get; set; }
        public StoredFileResponse? PdfInvoice { get; set; }
        public CityResponse? City { get; set; }
        public LogisticsProviderResponse? LogisticsProvider { get; set; }
        public string State { get; set; }
        public IEnumerable<SaleDetailResponse> SaleDetails { get; set; }
    }
}
