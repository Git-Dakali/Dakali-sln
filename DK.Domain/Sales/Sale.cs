using Dakali.Domine;
using Dakali.Domine.Base;
using DK.Domain.GeographicLocation;
using System;
using System.Collections.Generic;

namespace DK.Domain.Sales
{
    public enum SaleState
    { 
        Creado = 1,
        Confirmado = 2,
        AsignadoRuta = 4,
        EnViaje = 5,
        Entregado = 6,
        PendienteFacturar = 7,
        Facturado = 8,
        Finalizado = 9,
        Devuelto = 10,
        Cancelado = 11,
        Rechazado = 12,
    }

    public class Sale : EntityGuid
    {
        public string Identifier { get; set; }
        public long Number { get; set; }
        public string ArcaNumber { get; set; }
        public string Dni { get; set; }
        public string Cuit { get; set; }
        public DateTime Date { get; set; }
        public DateTime DeliveryDate { get; set; }
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
        public TaxStatus TaxStatus { get; set; }
        public OriginSale OriginSale { get; set; }
        public StoredFile PdfInvoice { get; set; }
        public City City { get; set; }
        public SaleState State { get; set; }
        public IEnumerable<SaleDetail> SaleDetails { get; set; }
    }
}
