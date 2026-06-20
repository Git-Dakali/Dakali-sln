using Dakali.Domine;
using System;
using System.Collections.Generic;

namespace DK.Domain.Sales
{
    public class SaleFilter : Filter
    {
        public string? Identifier { get; set; }
        public long? Number { get; set; }
        public string? ArcaNumber { get; set; }
        public string? Dni { get; set; }
        public string? Cuit { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? DeliveryDateFrom { get; set; }
        public DateTime? DeliveryDateTo { get; set; }
        public string? BusinessName { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public long? TaxStatusId { get; set; }
        public long? OriginSaleId { get; set; }
        public long? LogisticsProviderId { get; set; }
        public long? CityId { get; set; }
        public IEnumerable<string> Skus { get; set; }
        public IEnumerable<string> States { get; set; }
    }
}
