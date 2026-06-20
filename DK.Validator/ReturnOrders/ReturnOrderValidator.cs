using DK.Domain.ReturnOrders;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Validator.ReturnOrders
{
    public class ReturnOrderValidator
    {
        public async Task Create(ReturnOrder returnOrder, CancellationToken cancellationToken)
        { 
            if(returnOrder.Sale is null)
                throw new Exception($"La venta esta vacio.");
        }

        public async Task Returned(ReturnOrder returnOrder, CancellationToken cancellationToken)
        {
            if (returnOrder.State == ReturnOrderState.Devuelto)
                throw new Exception($"La Devolucion {returnOrder.Number} se encuentra en estado {ReturnOrderState.Devuelto.ToString()}.");
            if (returnOrder.State != ReturnOrderState.PendienteDevolver)
                throw new Exception($"La Devolucion {returnOrder.Number} de la Venta {returnOrder.Sale.Number} NO se encuentra en estado {ReturnOrderState.PendienteDevolver.ToString()}.");
        }

        public async Task Stored(ReturnOrder returnOrder, CancellationToken cancellationToken)
        {
            if (returnOrder.State == ReturnOrderState.Almacenado)
                throw new Exception($"La Devolucion {returnOrder.Number} se encuentra en estado {ReturnOrderState.Almacenado.ToString()}.");
            if (returnOrder.State != ReturnOrderState.Devuelto)
                throw new Exception($"La Devolucion {returnOrder.Number} NO se encuentra en estado {ReturnOrderState.Devuelto.ToString()}.");
        }

        public async Task NotReturned(ReturnOrder returnOrder, CancellationToken cancellationToken)
        {
            if (returnOrder.State == ReturnOrderState.NoDevuelto)
                throw new Exception($"La Devolucion {returnOrder.Number} se encuentra en estado {ReturnOrderState.NoDevuelto.ToString()}.");
            if (returnOrder.State != ReturnOrderState.PendienteDevolver)
                throw new Exception($"La Devolucion {returnOrder.Number} NO se encuentra en estado {ReturnOrderState.PendienteDevolver.ToString()}.");
        }
    }
}
