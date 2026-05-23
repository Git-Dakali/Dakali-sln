using DK.Domain.Sales;
using DK.Repositories.RoadMaps;
using DK.Repositories.Sales;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Validator.Sales
{
    public class SaleValidator
    {
        public SaleRepository _saleRepository;
        public RoadMapRepository _roadMapRepository;
        public SaleDetailValidator _saleDetailValidator;

        public SaleValidator(SaleRepository saleRepository, SaleDetailValidator saleDetailValidator, RoadMapRepository roadMapRepository)
        {
            _saleRepository = saleRepository ?? throw new ArgumentNullException("SaleRepository");
            _saleDetailValidator = saleDetailValidator ?? throw new ArgumentNullException("SaleDetailValidator");
            _roadMapRepository = roadMapRepository ?? throw new ArgumentNullException("RoadMapRepository");
        }

        public async Task Create(Sale sale, CancellationToken cancellationToken = default)
        {
            await Number(sale, cancellationToken);
            await ArcaNumber(sale, cancellationToken);
            await Identifier(sale, cancellationToken);
            await OriginSale(sale, cancellationToken);
            await Date(sale, cancellationToken);
            await DeliveryDate(sale, cancellationToken);
            await DeliveryStartTime(sale, cancellationToken);
            await DeliveryEndTime(sale, cancellationToken);
            await TotalPrice(sale, cancellationToken);
            await Observation(sale, cancellationToken);
            await BusinessName(sale, cancellationToken);
            await City(sale, cancellationToken);
            await Address(sale, cancellationToken);
            await Floor(sale, cancellationToken);
            await Apartment(sale, cancellationToken);
            await Phone(sale, cancellationToken);
            await Dni(sale, cancellationToken);
            await Cuit(sale, cancellationToken);
            await PdfInvoice(sale, cancellationToken);
            await State(sale, cancellationToken);
            await SaleDetails(sale, cancellationToken);

            foreach (var item in sale.SaleDetails)
                await _saleDetailValidator.Create(sale, item, cancellationToken);
        }

        public async Task Update(Sale sale, CancellationToken cancellationToken = default)
        {
            await Number(sale, cancellationToken);
            await ArcaNumber(sale, cancellationToken);
            await Identifier(sale, cancellationToken);
            await OriginSale(sale, cancellationToken);
            await Date(sale, cancellationToken);
            await DeliveryDate(sale, cancellationToken);
            await DeliveryStartTime(sale, cancellationToken);
            await DeliveryEndTime(sale, cancellationToken);
            await TotalPrice(sale, cancellationToken);
            await Observation(sale, cancellationToken);
            await BusinessName(sale, cancellationToken);
            await City(sale, cancellationToken);
            await Address(sale, cancellationToken);
            await Floor(sale, cancellationToken);
            await Apartment(sale, cancellationToken);
            await Phone(sale, cancellationToken);
            await Dni(sale, cancellationToken);
            await Cuit(sale, cancellationToken);
            await PdfInvoice(sale, cancellationToken);
            await State(sale, cancellationToken);
            await SaleDetails(sale, cancellationToken);

            foreach (var item in sale.SaleDetails)
                await _saleDetailValidator.Create(sale, item, cancellationToken);
        }

        public async Task Delete(Sale sale, CancellationToken cancellationToken = default)
        {
            if (!(await Exist(sale, cancellationToken)))
                throw new Exception($"No existe la Venta {sale.Number}");

            foreach (var item in sale.SaleDetails)
                await _saleDetailValidator.Delete(sale, item, cancellationToken);
        }

        public async Task Confirm(Sale sale, CancellationToken cancellationToken)
        {
            if (sale.State == SaleState.Confirmado)
                throw new Exception($"La venta {sale.Number} se encuentra en estado {SaleState.Confirmado.ToString()}.");
        }

        public async Task Prepared(Sale sale, CancellationToken cancellationToken)
        {
            if (sale.State == SaleState.Preparado)
                throw new Exception($"La venta {sale.Number} se encuentra en estado {SaleState.Preparado.ToString()}.");
            if (sale.State != SaleState.Confirmado)
                throw new Exception($"La venta {sale.Number} NO se encuentra en estado {SaleState.Confirmado.ToString()}.");
        }

        public async Task PendingDispatch(Sale sale, CancellationToken cancellationToken)
        {
            if (sale.State == SaleState.PendienteDespachar)
                throw new Exception($"La venta {sale.Number} se encuentra en estado {SaleState.PendienteDespachar.ToString()}.");
            if (sale.State != SaleState.Preparado)
                throw new Exception($"La venta {sale.Number} NO se encuentra en estado {SaleState.Preparado.ToString()}.");
        }

        public async Task Annular(Sale sale, CancellationToken cancellationToken)
        {
            if (sale.State == SaleState.Creado)
                return;
            if (sale.State == SaleState.Confirmado)
                return;
            if (sale.State == SaleState.Anulado)
                throw new Exception($"La venta {sale.Number} se encuentra en estado {SaleState.Anulado.ToString()}.");

            throw new Exception($"La venta {sale.Number} NO se encuentra en estado {SaleState.Creado.ToString()} o {SaleState.Confirmado.ToString()}.");
        }

        public async Task OnTrip(Sale sale, CancellationToken cancellationToken = default)
        {
            if (sale.State == SaleState.EnViaje)
                throw new Exception($"La venta {sale.Number} se encuentra en estado {SaleState.EnViaje.ToString()}.");

            if (sale.Latitude == 0 || sale.Longitude == 0)
                throw new Exception($"La venta {sale.Number} no se enceuntra geolocalizado.");

            if(sale.State != SaleState.PendienteDespachar)
                throw new Exception($"La venta {sale.Number} NO se enceuntra en estado {SaleState.PendienteDespachar}.");

            await SaleDetails(sale, cancellationToken);
        }

        public async Task PartialDeliver(Sale sale, CancellationToken cancellationToken)
        {
            if (sale.State == SaleState.EntregadoParcial)
                throw new Exception($"La venta {sale.Number} se encuentra en estado {SaleState.EntregadoParcial.ToString()}.");
            if (sale.State != SaleState.EnViaje)
                throw new Exception($"La venta {sale.Number} NO se encuentra en estado {SaleState.EnViaje.ToString()}.");
        }

        public async Task Deliver(Sale sale, CancellationToken cancellationToken)
        {
            if (sale.State == SaleState.Entregado)
                throw new Exception($"La venta {sale.Number} se encuentra en estado {SaleState.Entregado.ToString()}.");
            if (sale.State != SaleState.EnViaje)
                throw new Exception($"La venta {sale.Number} NO se encuentra en estado {SaleState.EnViaje.ToString()}.");
        }

        public async Task Reject(Sale sale, CancellationToken cancellationToken)
        {
            if (sale.State == SaleState.Rechazado)
                throw new Exception($"La venta {sale.Number} se encuentra en estado {SaleState.Rechazado.ToString()}.");
            if (sale.State != SaleState.EnViaje)
                throw new Exception($"La venta {sale.Number} NO se encuentra en estado {SaleState.EnViaje.ToString()}.");
        }

        public async Task Cancel(Sale sale, CancellationToken cancellationToken)
        {
            if (sale.State == SaleState.Cancelado)
                throw new Exception($"La venta {sale.Number} se encuentra en estado {SaleState.Cancelado.ToString()}.");
            if (sale.State != SaleState.Preparado && sale.State != SaleState.PendienteDespachar)
                throw new Exception($"La venta {sale.Number} NO se encuentra en estado {SaleState.Preparado.ToString()} o {SaleState.PendienteDespachar.ToString()}.");
        }

        public async Task Return(Sale sale, CancellationToken cancellationToken)
        {
            if (sale.State == SaleState.Devuelto)
                throw new Exception($"La venta {sale.Number} se encuentra en estado {SaleState.Devuelto.ToString()}.");
            if (sale.State != SaleState.Rechazado && sale.State != SaleState.Cancelado)
                throw new Exception($"La venta {sale.Number} NO se encuentra en estado {SaleState.Rechazado.ToString()} o {SaleState.Cancelado.ToString()}.");
        }

        public async Task Stored(Sale sale, CancellationToken cancellationToken)
        {
            if (sale.State == SaleState.Almacenado)
                throw new Exception($"La venta {sale.Number} se encuentra en estado {SaleState.Almacenado.ToString()}.");
            if (sale.State != SaleState.Devuelto)
                throw new Exception($"La venta {sale.Number} NO se encuentra en estado {SaleState.Devuelto.ToString()}.");
        }

        public async Task PendingBilling(Sale sale, CancellationToken cancellationToken)
        {
            if (sale.State == SaleState.PendienteFacturar)
                throw new Exception($"La venta {sale.Number} se encuentra en estado {SaleState.PendienteFacturar.ToString()}.");
            if (sale.State != SaleState.Entregado && sale.State != SaleState.EntregadoParcial)
                throw new Exception($"La venta {sale.Number} NO se encuentra en estado {SaleState.Entregado.ToString()} o {SaleState.EntregadoParcial.ToString()}.");
        }

        public async Task Invoiced(Sale sale, CancellationToken cancellationToken)
        {
            if (sale.State == SaleState.Facturado)
                throw new Exception($"La venta {sale.Number} se encuentra en estado {SaleState.Facturado.ToString()}.");
            if (sale.State != SaleState.PendienteFacturar)
                throw new Exception($"La venta {sale.Number} NO se encuentra en estado {SaleState.PendienteFacturar.ToString()}.");
        }

        public async Task<bool> Exist(Sale sale, CancellationToken cancellationToken = default)
        {
            return (await _saleRepository.Get(sale.Id, cancellationToken)) != null;
        }

        public async Task Identifier(Sale sale, CancellationToken cancellationToken = default) 
        {
            
        }

        public async Task Dni(Sale sale, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(sale.Dni))
                return;

            if (sale.Dni.Length < 7 || sale.Dni.Length > 8)
                throw new Exception("El DNI debe ser de 7 o 8 numeros");

            if(!sale.Dni.All(char.IsDigit))
                throw new Exception("El DNI debe ser solo numeros.");
        }

        public async Task Cuit(Sale sale, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(sale.Cuit))
                return;

            if (sale.Cuit.Length != 11)
                throw new Exception("El CUIT debe ser de 11 numeros");

            if (!sale.Dni.All(char.IsDigit))
                throw new Exception("El CUIT debe ser solo numeros.");
        }

        public async Task Number (Sale sale, CancellationToken cancellationToken = default) { }
        
        public async Task ArcaNumber (Sale sale, CancellationToken cancellationToken = default) { }
        
        public async Task Date (Sale sale, CancellationToken cancellationToken = default) 
        {
            if (sale.Id != 0)
                return;

            if (sale.Date == null)
                throw new Exception("Debe ungresar una fecha");

            if (sale.Date == DateTime.MinValue)
                throw new Exception("Debe ungresar una fecha");
        }
        
        public async Task DeliveryDate (Sale sale, CancellationToken cancellationToken = default) 
        {
            if (sale.DeliveryDate == null)
                throw new Exception("Debe ingresar una fecha de entrega");

            if (sale.DeliveryDate == DateTime.MinValue)
                throw new Exception("Debe ingresar una fecha de entrega");

            if (sale.DeliveryDate < DateTime.Today)
                throw new Exception("Debe ingresar una fecha de entrega mayor a la actual");
        }
        
        public async Task DeliveryStartTime (Sale sale, CancellationToken cancellationToken = default) { }
        
        public async Task DeliveryEndTime (Sale sale, CancellationToken cancellationToken = default) { }
        
        public async Task BusinessName (Sale sale, CancellationToken cancellationToken = default) 
        {
            if (string.IsNullOrWhiteSpace(sale.BusinessName))
                throw new Exception("Debe ingresar una Razon Social.");
        }
        
        public async Task Address (Sale sale, CancellationToken cancellationToken = default) 
        {
            if (string.IsNullOrWhiteSpace(sale.Address))
                throw new Exception("Debe ingresar un Domicilio.");
        }
        
        public async Task Floor (Sale sale, CancellationToken cancellationToken = default) { }
        
        public async Task Apartment (Sale sale, CancellationToken cancellationToken = default) { }
        
        public async Task Phone (Sale sale, CancellationToken cancellationToken = default) 
        {
            if (string.IsNullOrWhiteSpace(sale.Phone))
                throw new Exception("Debe ingresar un Telefono.");
        }
        
        public async Task Observation (Sale sale, CancellationToken cancellationToken = default) { }
        
        public async Task TotalPrice (Sale sale, CancellationToken cancellationToken = default) { }
        
        public async Task OriginSale (Sale sale, CancellationToken cancellationToken = default) 
        {
            if (sale.OriginSale is null)
                throw new Exception("Debe ingresar un Origen de Venta");
        }
        
        public async Task PdfInvoice (Sale sale, CancellationToken cancellationToken = default) { }
        
        public async Task City (Sale sale, CancellationToken cancellationToken = default) 
        {
            if (sale.City is null)
                throw new Exception("Debe ingresar una Localidad.");
        }
        
        public async Task State (Sale sale, CancellationToken cancellationToken = default) { }
        
        public async Task SaleDetails(Sale sale, CancellationToken cancellationToken = default) 
        {
            if (sale.SaleDetails is null)
                throw new Exception("De ingresar un detalle");

            if (sale.SaleDetails.Count() == 0)
                throw new Exception("De ingresar un detalle");

            if (sale.SaleDetails.Count(x => !x.IsExtra) == 0)
                throw new Exception("De ingresar un detalle que no sea extra.");
        }
    }
}
