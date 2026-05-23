using Dakali.Domine;
using DK.Domain.Locations;
using DK.Domain.RoadMaps;
using DK.Domain.Sales;
using DK.Process.Locations;
using DK.Process.Product;
using DK.Process.RoadMaps;
using DK.Repositories.Sales;
using DK.Validator.RoadMaps;
using DK.Validator.Sales;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Process.Sales
{
    public class SaleProcess
    {
        private SaleRepository _saleRepository;
        private SaleValidator _saleValidator;
        private SaleDetailProcess _saleDetailProcess;
        private StockProcess _stockProcess;
        private LocationStateProcess _locationStateProcess;
        private HistoricSaleProcess _historicSaleProcess;
        private IServiceProvider _serviceProvider;

        public SaleProcess(IServiceProvider serviceProvider, SaleRepository saleRepository, SaleValidator saleValidator, SaleDetailProcess saleDetailProcess, StockProcess stockProcess, 
            LocationStateProcess locationStateProcess, HistoricSaleProcess historicSaleProcess)
        {
            _saleRepository = saleRepository;
            _saleValidator = saleValidator;
            _saleDetailProcess = saleDetailProcess;
            _stockProcess = stockProcess;
            _locationStateProcess = locationStateProcess;
            _historicSaleProcess = historicSaleProcess;
            _serviceProvider = serviceProvider;
        }

        public async Task<IEnumerable<Sale>> GetAll(CancellationToken cancellationToken = default)
        {
            return await _saleRepository.GetAll(cancellationToken);
        }

        public async Task<ResultPage<Sale>> GetPage(SaleFilter saleFilter, CancellationToken cancellationToken = default)
        {
            return await _saleRepository.GetPage(saleFilter, cancellationToken);
        }

        public async Task<Sale> Get(long id, CancellationToken cancellationToken = default)
        {
            return await _saleRepository.Get(id, cancellationToken);
        }

        public async Task<Sale> GetByNumber(long number, CancellationToken cancellationToken = default)
        {
            return await _saleRepository.GetByNumber(number, cancellationToken);
        }

        public async Task<Sale> Create(Sale sale, CancellationToken cancellationToken = default)
        {
            await _saleValidator.Create(sale, cancellationToken);

            var saleResult = await _saleRepository.Create(sale, cancellationToken);
            var state = await _locationStateProcess.Get("DIS", cancellationToken);
            
            foreach (var detail in saleResult.SaleDetails) 
                await Reserve(state, saleResult, detail, cancellationToken);

            var saleNew = await _saleRepository.Get(saleResult.Id, cancellationToken);

            await _historicSaleProcess.Create(saleNew, $"Se creo la venta {saleNew.Number}", cancellation: cancellationToken);

            return saleNew;
        }

        public async Task<Sale> Update(Sale product, CancellationToken cancellationToken = default)
        {
            await _saleValidator.Update(product, cancellationToken);

            return await _saleRepository.Update(product, cancellationToken);
        }

        public async Task AddLocation(Sale entity, CancellationToken cancellation = default)
        {
            await _saleRepository.AddLocation(entity, cancellation);
        }

        public async Task Delete(Sale sale, CancellationToken cancellationToken = default)
        {
            var roadMapProcess = _serviceProvider.GetService<RoadMapProcess>();
            var roadMapSaleProcess = _serviceProvider.GetService<RoadMapSaleProcess>();
            var roadMapSaleValidator = _serviceProvider.GetService<RoadMapSaleValidator>();

            await _saleValidator.Delete(sale, cancellationToken);
            var roadMap = await roadMapProcess?.Get(sale, cancellationToken);
            
            if (roadMap != null)
            {
                var roadMapSale = await roadMapSaleProcess.Get(roadMap, sale, cancellationToken);
                await roadMapSaleValidator.UnassignRoadMap(roadMap, roadMapSale, cancellationToken);
                await roadMapSaleProcess.UnassignRoadMap(roadMap, roadMapSale, cancellationToken);
            }

            foreach (var detail in sale.SaleDetails)
            {
                await _stockProcess.CancelReserve(detail.Stock, detail.Count, cancellationToken);
                await _saleDetailProcess.UnassignStock(sale, detail, cancellationToken);
            }

            await _saleRepository.Delete(sale, cancellationToken);
            await _historicSaleProcess.Create(sale, $"Se elimino la venta {sale.Number}", cancellation: cancellationToken);
        }

        private async Task Reserve(LocationState state, Sale sale, SaleDetail detail, CancellationToken cancellation)
        {
            var stock = await _stockProcess.Reserve(state, detail.Product, detail.Variant, detail.Color, detail.Count, cancellation);
            await _saleDetailProcess.AssignStock(sale, detail, stock, cancellation);
        }

        public async Task<Sale> Confirm(Sale sale, CancellationToken cancellationToken)
        {
            var salePersisted = await _saleRepository.Get(sale.Id, cancellationToken);

            await _saleValidator.Confirm(salePersisted, cancellationToken);
            await _saleRepository.UpdateState(salePersisted.Id, SaleState.Confirmado, cancellationToken);

            await _historicSaleProcess.Create(sale, $"Se confirmo para preparar", cancellation: cancellationToken);
            return await _saleRepository.Get(salePersisted.Id, cancellationToken);
        }

        public async Task<Sale> Prepared(Sale sale, CancellationToken cancellationToken)
        {
            var salePersisted = await _saleRepository.Get(sale.Id, cancellationToken);

            await _saleValidator.Prepared(salePersisted, cancellationToken);
            await _saleRepository.UpdateState(salePersisted.Id, SaleState.Preparado, cancellationToken);

            await _historicSaleProcess.Create(sale, $"Se preparo", cancellation: cancellationToken);
            return await _saleRepository.Get(salePersisted.Id, cancellationToken);
        }

        public async Task<Sale> PendingDispatch(Sale sale, CancellationToken cancellationToken)
        {
            var salePersisted = await _saleRepository.Get(sale.Id, cancellationToken);

            await _saleValidator.PendingDispatch(salePersisted, cancellationToken);
            await _saleRepository.UpdateState(salePersisted.Id, SaleState.PendienteDespachar, cancellationToken);

            await _historicSaleProcess.Create(sale, $"Listo para despachar", cancellation: cancellationToken);
            return await _saleRepository.Get(salePersisted.Id, cancellationToken);
        }

        public async Task<Sale> OnTrip(Sale sale, CancellationToken cancellation)
        {
            var salePersisted = await _saleRepository.Get(sale.Id, cancellation);

            await _saleValidator.OnTrip(salePersisted, cancellation);
            await _saleRepository.UpdateState(salePersisted.Id, SaleState.EnViaje, cancellation);

            await _historicSaleProcess.Create(sale, $"Se encuentra en viaje", cancellation: cancellation);

            return await _saleRepository.Get(salePersisted.Id, cancellation);
        }

        public async Task<Sale> Deliver(Sale sale, CancellationToken cancellationToken)
        {
            var salePersisted = await _saleRepository.Get(sale.Id, cancellationToken);

            await _saleValidator.Deliver(salePersisted, cancellationToken);
            await _saleRepository.UpdateState(salePersisted.Id, SaleState.Entregado, cancellationToken);

            await _historicSaleProcess.Create(sale, $"Entregado", cancellation: cancellationToken);
            return await _saleRepository.Get(salePersisted.Id, cancellationToken);
        }

        public async Task<Sale> PartialDelivered(Sale sale, CancellationToken cancellationToken)
        {
            var salePersisted = await _saleRepository.Get(sale.Id, cancellationToken);

            await _saleValidator.PartialDeliver(salePersisted, cancellationToken);
            await _saleRepository.UpdateState(salePersisted.Id, SaleState.EntregadoParcial, cancellationToken);

            await _historicSaleProcess.Create(sale, $"Entregado Parcial", cancellation: cancellationToken);
            return await _saleRepository.Get(salePersisted.Id, cancellationToken);
        }

        public async Task<Sale> Reject(Sale sale, CancellationToken cancellationToken)
        {
            var salePersisted = await _saleRepository.Get(sale.Id, cancellationToken);

            await _saleValidator.Reject(salePersisted, cancellationToken);
            await _saleRepository.UpdateState(salePersisted.Id, SaleState.Rechazado, cancellationToken);

            await _historicSaleProcess.Create(sale, $"Rechazado por el cliente", cancellation: cancellationToken);
            return await _saleRepository.Get(salePersisted.Id, cancellationToken);
        }

        public async Task<Sale> Cancel(Sale sale, CancellationToken cancellationToken)
        {
            var salePersisted = await _saleRepository.Get(sale.Id, cancellationToken);

            await _saleValidator.Cancel(salePersisted, cancellationToken);
            await _saleRepository.UpdateState(salePersisted.Id, SaleState.Cancelado, cancellationToken);

            await _historicSaleProcess.Create(sale, $"Cancelado por el cliente", cancellation: cancellationToken);
            return await _saleRepository.Get(salePersisted.Id, cancellationToken);
        }

        public async Task<Sale> Return(Sale sale, CancellationToken cancellationToken)
        {
            var salePersisted = await _saleRepository.Get(sale.Id, cancellationToken);

            await _saleValidator.Return(salePersisted, cancellationToken);
            await _saleRepository.UpdateState(salePersisted.Id, SaleState.Devuelto, cancellationToken);

            await _historicSaleProcess.Create(sale, $"Devuelto", cancellation: cancellationToken);
            return await _saleRepository.Get(salePersisted.Id, cancellationToken);
        }

        public async Task<Sale> Stored(Sale sale, CancellationToken cancellationToken)
        {
            var salePersisted = await _saleRepository.Get(sale.Id, cancellationToken);

            await _saleValidator.Stored(salePersisted, cancellationToken);
            await _saleRepository.UpdateState(salePersisted.Id, SaleState.Almacenado, cancellationToken);

            await _historicSaleProcess.Create(sale, $"Se Almaceno.", cancellation: cancellationToken);
            return await _saleRepository.Get(salePersisted.Id, cancellationToken);
        }

        public async Task<Sale> PendingBilling(Sale sale, CancellationToken cancellationToken)
        {
            var salePersisted = await _saleRepository.Get(sale.Id, cancellationToken);

            await _saleValidator.PendingBilling(salePersisted, cancellationToken);
            await _saleRepository.UpdateState(salePersisted.Id, SaleState.PendienteFacturar, cancellationToken);

            await _historicSaleProcess.Create(sale, $"Pendiente de facturar", cancellation: cancellationToken);
            return await _saleRepository.Get(salePersisted.Id, cancellationToken);
        }

        public async Task<Sale> Invoiced(Sale sale, CancellationToken cancellationToken)
        {
            var salePersisted = await _saleRepository.Get(sale.Id, cancellationToken);

            await _saleValidator.Invoiced(salePersisted, cancellationToken);
            await _saleRepository.UpdateState(salePersisted.Id, SaleState.Facturado, cancellationToken);

            await _historicSaleProcess.Create(sale, $"Facturado", cancellation: cancellationToken);
            return await _saleRepository.Get(salePersisted.Id, cancellationToken);
        }
    }
}
