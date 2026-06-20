using Dakali.Domine.Base;
using DK.Domain.Sales;
using System;

namespace DK.Domain.ReturnOrders
{
    public enum ReturnOrderState
    {
        PendienteDevolver = 1,
        Devuelto = 2,
        Almacenado = 3,
        NoDevuelto = 4,
    }

    public class ReturnOrder : EntityGuid
    {
        public long Number { get; set; }
        public Sale Sale { get; set; }
        public DateTime? ReturnDate { get; set; }
        public ReturnOrderState State { get; set; }
    }
}
