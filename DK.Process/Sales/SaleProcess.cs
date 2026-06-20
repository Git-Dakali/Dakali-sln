using Dakali.Domine;
using DK.Domain.Locations;
using DK.Domain.ReturnOrders;
using DK.Domain.Sales;
using DK.Process.Locations;
using DK.Process.Product;
using DK.Process.ReturnOrders;
using DK.Process.RoadMaps;
using DK.Repositories.Sales;
using DK.Validator.RoadMaps;
using DK.Validator.Sales;
using Microsoft.Extensions.DependencyInjection;
using NPOI.HSSF.UserModel; // XLS
using System;
using System.Collections.Generic;
using System.IO;
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

        public async Task<byte[]> GetReportExcelDarLogitics(IEnumerable<long> ids, CancellationToken cancellationToken = default)
        {
            var values = await _saleRepository.GetReportExcelDarLogitics(ids, cancellationToken);

            // Crear workbook XLS
            var workbook = new HSSFWorkbook();
            var sheet = workbook.CreateSheet("Reporte");

            // Crear formato TEXTO
            var textFormat = workbook.CreateDataFormat();
            var textStyle = workbook.CreateCellStyle();
            textStyle.DataFormat = textFormat.GetFormat("@");

            // Aplicar el estilo a TODA la columna (ej: columna 0)
            sheet.SetDefaultColumnStyle(1, textStyle);

            // Crear fila de encabezado
            var header = sheet.CreateRow(0);
            header.CreateCell(0).SetCellValue("Numero de tracking");
            header.CreateCell(1).SetCellValue("Fecha de venta");
            header.CreateCell(2).SetCellValue("Valor declarado");
            header.CreateCell(3).SetCellValue("Peso declarado");
            header.CreateCell(4).SetCellValue("Destinatario");
            header.CreateCell(5).SetCellValue("Teléfono de contacto");
            header.CreateCell(6).SetCellValue("Dirección");
            header.CreateCell(7).SetCellValue("Localidad");
            header.CreateCell(8).SetCellValue("Código postal");
            header.CreateCell(9).SetCellValue("Observaciones");
            header.CreateCell(10).SetCellValue("4 Total a cobrar");
            header.CreateCell(11).SetCellValue("1 Logistica Inversa");

            var rowNumber = 1;
            foreach (var item in values)
            {
                var peso = "";
                if (item.Peso < 1000)
                    peso = $"{item.Peso}g";
                else
                    peso = $"{(item.Peso / (decimal)1000.0).ToString("0.#")}kg";
                
                // Crear fila con datos
                var row = sheet.CreateRow(rowNumber);
                row.CreateCell(0).SetCellValue(item.Tracking);
                row.CreateCell(1).SetCellValue(item.FechaEntrega);
                row.CreateCell(2).SetCellValue((double)item.ValorDeclarado);
                row.CreateCell(3).SetCellValue(peso);
                row.CreateCell(4).SetCellValue(item.Destinatario);
                row.CreateCell(5).SetCellValue(item.Telefono);
                row.CreateCell(6).SetCellValue(item.Direccion);
                row.CreateCell(7).SetCellValue(item.Localidad);
                row.CreateCell(8).SetCellValue(item.CodigoPostal);
                row.CreateCell(9).SetCellValue(item.Observacion);
                row.CreateCell(10).SetCellValue((double)item.PrecioTotal);
                row.CreateCell(11).SetCellValue(item.LogisticaInversa);
                rowNumber++;
            }


            // Guardar en memoria
            using var ms = new MemoryStream();
            workbook.Write(ms);
            ms.Position = 0;

            return ms.ToArray();
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

        public async Task<Sale> Update(Sale sale, CancellationToken cancellationToken = default)
        {
            await _saleValidator.Update(sale, cancellationToken);
            var salePersisted = await _saleRepository.Update(sale, cancellationToken);

            if (salePersisted.State == SaleState.Preparado || salePersisted.State == SaleState.PendienteDespachar)
            {
                sale.State = SaleState.Confirmado;

                await Confirm(sale, cancellationToken);

                await _historicSaleProcess.Create(sale, $"Se actualizo la venta.", cancellation: cancellationToken);
                return await _saleRepository.Get(salePersisted.Id, cancellationToken);
            }
            else 
            {
                var state = await _locationStateProcess.Get("DIS", cancellationToken);

                foreach (var detail in salePersisted.SaleDetails)
                {
                    if (detail.Stock is null)
                        await Reserve(state, salePersisted, detail, cancellationToken);
                }

                return salePersisted;
            }    
        }

        public async Task UpdateIsPrinted(long saleId, bool isPrinted, CancellationToken cancellation = default)
        {
            await _saleRepository.UpdateIsPrinted(saleId, isPrinted, cancellation);
        }

        public async Task AddLocation(Sale entity, CancellationToken cancellation = default)
        {
            await _saleRepository.AddLocation(entity, cancellation);
        }

        public async Task Delete(Sale sale, CancellationToken cancellationToken = default)
        {
            var salePersisted = await Get(sale.Id, cancellationToken);
            var roadMapProcess = _serviceProvider.GetService<RoadMapProcess>();
            var roadMapSaleProcess = _serviceProvider.GetService<RoadMapSaleProcess>();
            var roadMapSaleValidator = _serviceProvider.GetService<RoadMapSaleValidator>();

            await _saleValidator.Delete(salePersisted, cancellationToken);
            var roadMap = await roadMapProcess?.Get(salePersisted, cancellationToken);
            
            if (roadMap != null)
            {
                var roadMapSale = await roadMapSaleProcess.Get(roadMap, salePersisted, cancellationToken);
                await roadMapSaleValidator.UnassignRoadMap(roadMap, roadMapSale, cancellationToken);
                await roadMapSaleProcess.UnassignRoadMap(roadMap, roadMapSale, cancellationToken);
            }

            foreach (var detail in salePersisted.SaleDetails)
            {
                await _stockProcess.CancelReserve(detail.Stock, detail.Count, cancellationToken);
                await _saleDetailProcess.UnassignStock(salePersisted, detail, cancellationToken);
            }

            await _saleRepository.UpdateState(salePersisted.Id, SaleState.Anulado, cancellationToken);
            await _historicSaleProcess.Create(salePersisted, $"Se elimino la venta {salePersisted.Number}", cancellation: cancellationToken);
        }

        private async Task Reserve(LocationState state, Sale sale, SaleDetail detail, CancellationToken cancellation)
        {
            var stock = await _stockProcess.Reserve(state, detail.Product, detail.ProductSku, detail.Count, cancellation);
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

            foreach (var detail in salePersisted.SaleDetails)
                await _stockProcess.Commit(detail.Stock, detail.Count, cancellation);

            await _historicSaleProcess.Create(sale, $"Se encuentra en viaje", cancellation: cancellation);

            return await _saleRepository.Get(salePersisted.Id, cancellation);
        }

        public async Task<Sale> Deliver(Sale sale, CancellationToken cancellationToken)
        {
            var salePersisted = await _saleRepository.Get(sale.Id, cancellationToken);

            await _saleValidator.Deliver(salePersisted, cancellationToken);
            await _saleRepository.UpdateState(salePersisted.Id, SaleState.Entregado, cancellationToken);

            if (sale.IsReverseLogistics)
            {
                var returnOrderProcess = _serviceProvider.GetService<ReturnOrderProcess>();
                await returnOrderProcess.Create(new ReturnOrder() { Sale = sale }, cancellationToken);
            }
                
            await _historicSaleProcess.Create(sale, $"Entregado", cancellation: cancellationToken);
            return await _saleRepository.Get(salePersisted.Id, cancellationToken);
        }

        public async Task<Sale> PartialDelivered(Sale sale, CancellationToken cancellationToken)
        {
            var salePersisted = await _saleRepository.Get(sale.Id, cancellationToken);

            await _saleValidator.PartialDeliver(salePersisted, cancellationToken);
            await _saleRepository.UpdateState(salePersisted.Id, SaleState.EntregadoParcial, cancellationToken);

            var returnOrderProcess = _serviceProvider.GetService<ReturnOrderProcess>();
            await returnOrderProcess.Create(new ReturnOrder() { Sale = sale }, cancellationToken);

            await _historicSaleProcess.Create(sale, $"Entregado Parcial", cancellation: cancellationToken);
            return await _saleRepository.Get(salePersisted.Id, cancellationToken);
        }

        public async Task<Sale> Reject(Sale sale, CancellationToken cancellationToken)
        {
            var salePersisted = await _saleRepository.Get(sale.Id, cancellationToken);

            await _saleValidator.Reject(salePersisted, cancellationToken);
            await _saleRepository.UpdateState(salePersisted.Id, SaleState.Rechazado, cancellationToken);

            var returnOrderProcess = _serviceProvider.GetService<ReturnOrderProcess>();
            await returnOrderProcess.Create(new ReturnOrder() { Sale = sale }, cancellationToken);

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
    }
}
