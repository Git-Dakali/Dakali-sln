using DK.Domain.Sales;
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
        public SaleDetailValidator _saleDetailValidator;

        public SaleValidator(SaleRepository saleRepository, SaleDetailValidator saleDetailValidator)
        {
            _saleRepository = saleRepository ?? throw new ArgumentNullException("SaleRepository");
            _saleDetailValidator = saleDetailValidator ?? throw new ArgumentNullException("SaleDetailValidator");
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
                throw new Exception("Debe ungresar una fecha de entrega");

            if (sale.DeliveryDate == DateTime.MinValue)
                throw new Exception("Debe ungresar una fecha de entrega");

            if (sale.DeliveryDate < DateTime.Today)
                throw new Exception("Debe ungresar una fecha mayor a la actual");
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
