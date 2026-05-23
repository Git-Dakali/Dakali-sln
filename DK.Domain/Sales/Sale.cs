using Dakali.Domine;
using Dakali.Domine.Base;
using DK.Domain.GeographicLocation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DK.Domain.Sales
{
    public enum SaleState
    { 
        Creado = 1,
        Confirmado = 2,
        Preparado = 3,
        Anulado = 4,
        PendienteDespachar = 5,
        EnViaje = 6,
        Rechazado = 7,
        Entregado = 8,
        PendienteFacturar = 9,
        Facturado = 10,
        Devuelto = 11,
        Almacenado = 12,
        EntregadoParcial = 13,
        Cancelado = 14,
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
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public TaxStatus TaxStatus { get; set; }
        public OriginSale OriginSale { get; set; }
        public StoredFile PdfInvoice { get; set; }
        public City City { get; set; }
        public SaleState State { get; set; }
        public IEnumerable<SaleDetail> SaleDetails { get; set; }

        public override string ToString()
        {
            return $"{Identifier} {Number} {ArcaNumber} {Dni} {Cuit} Emision{Date.ToString("dd-MM-yyyy")} Entrega{DeliveryDate.ToString("dd-MM-yyyy")} {BusinessName} {Phone} {TaxStatus?.Code} {TaxStatus?.Name} {OriginSale?.Code} {OriginSale?.Name} {Address} {City?.ZipCode} {City?.Name} {State.ToString()} [ {string.Join(" , ", SaleDetails.Select(x => x.ToString()))} ]";
        }
    }
}
