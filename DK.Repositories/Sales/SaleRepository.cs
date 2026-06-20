using Dakali.Domine;
using Dakali.Interface.Connection;
using Dapper;
using DK.Domain.GeographicLocation;
using DK.Domain.Sales;
using DK.Domain.Sales.Report;
using DK.Repositories.GeographicLocation;
using DK.Repositories.Interface.Base;
using DK.Repositories.Products;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Repositories.Sales
{
    public class SaleRepository : IRepository<Sale>
    {
        private ISession _session;
        private TaxStatusRepository _taxStatusRepository;
        private OriginSaleRepository _originSaleRepository;
        private StoredFileRepository _storedFileRepository;
        private CityRepository _cityRepository;
        private LogisticsProviderRepository _logisticsProviderRepository;
        private SaleDetailRepository _saleDetailRepository;

        public SaleRepository(ISession session, TaxStatusRepository taxStatusRepository, OriginSaleRepository originSaleRepository, StoredFileRepository storedFileRepository, CityRepository cityRepository, LogisticsProviderRepository logisticsProviderRepository, SaleDetailRepository saleDetailRepository)
        {
            _session = session;
            _taxStatusRepository = taxStatusRepository;
            _originSaleRepository = originSaleRepository;
            _storedFileRepository = storedFileRepository;
            _cityRepository = cityRepository;
            _saleDetailRepository = saleDetailRepository;
            _logisticsProviderRepository = logisticsProviderRepository;
        }

        public async Task<Sale> Create(Sale entity, CancellationToken cancellation = default)
        {
            var query = @"
            INSERT INTO dbo.Sale (IsPrinted, IsReverseLogistics, Identifier, ArcaNumber, Dni, Cuit, Date, DeliveryDate, DeliveryStartTime, DeliveryEndTime, BusinessName, Address, Floor, Apartment, Phone, Observation, GrossPrice, Discounts, TotalPrice, ShippingPrice, TaxStatusId, OriginSaleId, PdfInvoiceId, CityId, LogisticsProviderId, State, SearchString)
            OUTPUT INSERTED.Id, INSERTED.SearchString, INSERTED.CreationDate, INSERTED.RemoveDate, INSERTED.UpdateDate, INSERTED.Version, INSERTED.Guid, INSERTED.IsDeleted, 
                INSERTED.IsPrinted, INSERTED.IsReverseLogistics, INSERTED.Identifier, INSERTED.Number, INSERTED.ArcaNumber, INSERTED.Date, INSERTED.DeliveryDate, INSERTED.DeliveryStartTime, 
                INSERTED.DeliveryEndTime, INSERTED.BusinessName, INSERTED.Dni, INSERTED.Cuit, INSERTED.Address, INSERTED.Floor, INSERTED.Apartment, INSERTED.Phone, INSERTED.Observation, 
                INSERTED.GrossPrice, INSERTED.ShippingPrice, INSERTED.Discounts, INSERTED.TotalPrice, INSERTED.TaxStatusId, INSERTED.OriginSaleId, INSERTED.PdfInvoiceId, INSERTED.CityId, INSERTED.LogisticsProviderId,
                INSERTED.State, INSERTED.Latitude, INSERTED.Longitude
            VALUES (@IsPrinted, @IsReverseLogistics, @Identifier, @ArcaNumber, @Dni, @Cuit, @Date, @DeliveryDate, @DeliveryStartTime, @DeliveryEndTime, @BusinessName, @Address, @Floor, @Apartment, @Phone, @Observation, @GrossPrice, @Discounts, @TotalPrice, @ShippingPrice, @TaxStatusId, @OriginSaleId, @PdfInvoiceId, @CityId, @LogisticsProviderId, @State, @SearchString);";
            entity.State = SaleState.Creado;
            entity.SearchString = entity.ToString();
            var rowDapper = await _session.Connection.QuerySingleAsync(new CommandDefinition(query, 
                new {
                    entity.IsPrinted,
                    entity.IsReverseLogistics,
                    Identifier = entity.Identifier ?? string.Empty, 
                    ArcaNumber = entity.ArcaNumber ?? string.Empty,
                    Dni = entity.Dni ?? string.Empty,
                    Cuit = entity.Cuit ?? string.Empty,
                    entity.Date, 
                    entity.DeliveryDate, 
                    entity.DeliveryStartTime, 
                    entity.DeliveryEndTime,
                    BusinessName = entity.BusinessName?? string.Empty, 
                    Address = entity.Address ?? string.Empty, 
                    Floor = entity.Floor ?? string.Empty, 
                    Apartment = entity.Apartment ?? string.Empty, 
                    Phone = entity.Phone ?? string.Empty,
                    Observation = entity.Observation ?? string.Empty,
                    entity.GrossPrice,
                    entity.Discounts,
                    entity.TotalPrice,
                    entity.ShippingPrice,
                    TaxStatusId = entity.TaxStatus?.Id,
                    OriginSaleId = entity.OriginSale?.Id,
                    PdfInvoiceId = entity.PdfInvoice?.Id,
                    CityId = entity.City?.Id,
                    LogisticsProviderId = entity.LogisticsProvider?.Id,
                    State = SaleState.Creado.ToString(),
                    SearchString = entity.SearchString ?? string.Empty 
                }, transaction: _session.Transaction, cancellationToken: cancellation));

            if (rowDapper is null)
                throw new Exception("No se pudo crear la venta");

            var sale = await Map(rowDapper);

            await _saleDetailRepository.SyncCollection(sale, entity.SaleDetails, cancellation);
            sale.SaleDetails = await _saleDetailRepository.Get(sale, cancellation);

            return sale;
        }

        public async Task Delete(Sale entity, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.Sale
                   SET IsDeleted = 1,
                       State = @State,
                       RemoveDate = SYSUTCDATETIME(),
                       UpdateDate = SYSUTCDATETIME(),
                       Version = Version + 1
                 WHERE Id = @Id AND IsDeleted = 0;";

            await _session.Connection.ExecuteAsync(new CommandDefinition(query, new { entity.Id, State = SaleState.Anulado }, transaction: _session.Transaction, cancellationToken: cancellation));
            await _saleDetailRepository.Delete(entity, entity.SaleDetails, cancellation);
        }

        public async Task<Sale> Get(long id, bool disableIsDeleted, CancellationToken cancellation = default)
        {
            var query = $@"
                select *
                from dbo.Sale 
                where {(disableIsDeleted ? string.Empty : "IsDeleted = 0 AND")} Id = @Id";

            var rowDapper = await _session.Connection.QuerySingleOrDefaultAsync(new CommandDefinition(query, new { Id = id }, transaction: _session.Transaction, cancellationToken: cancellation));

            if (rowDapper is null)
                return null;

            return await Map(rowDapper);
        }

        public async Task<Sale> Get(long id, CancellationToken cancellation = default)
        {
            return await Get(id, false, cancellation);
        }

        public async Task<IEnumerable<ExcelDarLogitics>> GetReportExcelDarLogitics(IEnumerable<long> ids, CancellationToken cancellation = default)
        {
            var query = $@"
                SELECT s.Id, s.Identifier as Tracking, CASE WHEN s.IsReverseLogistics = 1 THEN 'CAMBIO!!! ENTREGAR UN PAQUETE Y RECIBIR UN PAQUETE' ELSE '' END LogisticaInversa, FORMAT(s.DeliveryDate, 'dd/MM/yyyy') as FechaEntrega, s.BusinessName as Destinatario, s.Phone as Telefono, s.Address as Direccion, c.Name as Localidad, c.ZipCode as CodigoPostal, s.Observation as Observacion, s.TotalPrice as PrecioTotal, x.ValorDeclarado, x.Peso
                FROM Sale s
                LEFT JOIN City c on c.Id = s.CityId
                CROSS APPLY (
                    SELECT 
                        SUM(p.Price) AS ValorDeclarado,
                        SUM(p.Weight) AS Peso
                    FROM SaleDetail sd
                    INNER JOIN Product p ON p.Id = sd.ProductId
                    WHERE sd.SaleId = s.Id
                ) x
                WHERE s.Id in @Ids";

            return await _session.Connection.QueryAsync<ExcelDarLogitics>(new CommandDefinition(query, new { Ids = ids}, transaction: _session.Transaction, cancellationToken: cancellation));
        }

        public async Task<Sale> GetByNumber(long number, CancellationToken cancellation = default)
        {
            var query = @"
                select *
                from dbo.Sale 
                where IsDeleted = 0 AND Number = @Number";
            var rowDapper = await _session.Connection.QuerySingleOrDefaultAsync(new CommandDefinition(query, new { Number = number }, transaction: _session.Transaction, cancellationToken:cancellation));

            if (rowDapper is null)
                return null;

            return await Map(rowDapper);
        }

        public async Task<IEnumerable<Sale>> GetAll(CancellationToken cancellation = default)
        {
            var query = @"
                select *
                from dbo.Sale 
                where IsDeleted = 0";
            var rowsDapper = await _session.Connection.QueryAsync(new CommandDefinition(query, new { }, transaction: _session.Transaction, cancellationToken: cancellation));

            if (rowsDapper is null)
                return Enumerable.Empty<Sale>();

            var sales = new List<Sale>();

            foreach (var row in rowsDapper)
                sales.Add(await Map(row));

            return sales;
        }

        public async Task<ResultPage<Sale>> GetPage(SaleFilter saleFilter, CancellationToken cancellationToken = default)
        {
            if (saleFilter is null || saleFilter.CountRows <= 0 || saleFilter.Page <= 0)
                return new ResultPage<Sale>() { Count = 0, Values = new List<Sale>() };

            var query = @" select * from dbo.Sale where IsDeleted = 0";
            var queryCount = @" select COUNT(*) from dbo.Sale where IsDeleted = 0";
            var filterQuery = $"";

            dynamic filter = new ExpandoObject();

            if (saleFilter.Id != null)
            {
                filterQuery += " AND Id = @Id";
                filter.Id = saleFilter.Id;
            }

            if (!string.IsNullOrWhiteSpace(saleFilter.SearchString))
            {
                filterQuery += " AND CONTAINS(SearchString, @SearchString)";
                
                var values = (saleFilter.SearchString ?? string.Empty).Split(" ").Select(value => value.Trim()).Where(value => !string.IsNullOrWhiteSpace(value)).Select(value => $"\"{value}*\"");

                filter.SearchString = $"({string.Join(" AND ", values)})";
            }

            if ((saleFilter.OriginSaleId ??0) > 0)
            {
                filterQuery += " AND OriginSaleId = @OriginSaleId";
                filter.OriginSaleId = saleFilter.OriginSaleId;
            }

            if ((saleFilter.LogisticsProviderId ?? 0) > 0)
            {
                filterQuery += " AND LogisticsProviderId = @LogisticsProviderId";
                filter.LogisticsProviderId = saleFilter.LogisticsProviderId;
            }

            if ((saleFilter.Number ?? 0) > 0)
            {
                filterQuery += " AND Number = @Number";
                filter.Number = saleFilter.Number;
            }

            if (!string.IsNullOrWhiteSpace(saleFilter.Identifier))
            {
                filterQuery += " AND Identifier = @Identifier";
                filter.Identifier = saleFilter.Identifier;
            }

            if (saleFilter.DeliveryDateFrom != null && saleFilter.DeliveryDateFrom > new DateTime(2000, 1, 1))
            {
                filterQuery += " AND DeliveryDate >= @DeliveryDateFrom";
                filter.DeliveryDateFrom = saleFilter.DeliveryDateFrom?.ToString("yyyy-MM-dd");
            }

            if (saleFilter.DeliveryDateTo != null && saleFilter.DeliveryDateTo > new DateTime(2000, 1, 1))
            {
                filterQuery += " AND DeliveryDate <= @DeliveryDateTo";
                filter.DeliveryDateTo = saleFilter.DeliveryDateTo?.ToString("yyyy-MM-dd");
            }

            if (saleFilter.States.Any())
            {
                filterQuery += " AND State in @States";
                filter.States = saleFilter.States.Select(x=> x.ToString());
            }

            query += $" {filterQuery}";
            queryCount += $" {filterQuery}";

            query += @$"
                ORDER BY Id
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY;

                {queryCount}
            ";

            filter.Offset = (saleFilter.Page - 1) * saleFilter.CountRows;
            filter.PageSize = saleFilter.CountRows;

            var results = await _session.Connection.QueryMultipleAsync(new CommandDefinition(query, filter as object, transaction: _session.Transaction, cancellationToken: cancellationToken));
            var rowsDapper = results.Read().ToList();
            var count = results.Read<long>().Single();

            if (rowsDapper is null)
                return new ResultPage<Sale>() { Count = 0, Values = new List<Sale>() };

            var sales = new List<Sale>();

            foreach (var row in rowsDapper)
                sales.Add(await Map(row));

            return new ResultPage<Sale>() { Count = count, Values = sales };
        }

        public async Task<Sale> Update(Sale entity, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.Sale
                SET 
                    IsPrinted = @IsPrinted, 
                    IsReverseLogistics = @IsReverseLogistics, 
                    Identifier = @Identifier, 
                    ArcaNumber = @ArcaNumber,
                    Dni = @Dni, 
                    Cuit = @Cuit,
                    Date = @Date, 
                    DeliveryDate = @DeliveryDate, 
                    DeliveryStartTime = @DeliveryStartTime, 
                    DeliveryEndTime = @DeliveryEndTime, 
                    BusinessName = @BusinessName, 
                    Address = @Address, 
                    Floor = @Floor, 
                    Apartment = @Apartment, 
                    Phone = @Phone, 
                    Observation = @Observation,
                    GrossPrice = @GrossPrice,
                    Discounts = @Discounts,
                    TotalPrice = @TotalPrice, 
                    ShippingPrice = @ShippingPrice,
                    TaxStatusId = @TaxStatusId,
                    OriginSaleId = @OriginSaleId, 
                    PdfInvoiceId = @PdfInvoiceId, 
                    CityId = @CityId, 
                    LogisticsProviderId = @LogisticsProviderId,
                    SearchString = @SearchString,
                    UpdateDate = SYSUTCDATETIME(),
                    Version = Version + 1
                OUTPUT INSERTED.*
                WHERE Id = @Id AND IsDeleted = 0;
            ";

            entity.SearchString = entity.ToString();
            await _session.Connection.QuerySingleAsync<Sale>(new CommandDefinition(query,
                new
                {
                    entity.Id,
                    entity.IsPrinted,
                    entity.IsReverseLogistics,
                    Identifier = entity.Identifier ?? string.Empty,
                    ArcaNumber = entity.ArcaNumber ?? string.Empty,
                    Dni = entity.Dni ?? string.Empty,
                    Cuit = entity.Cuit ?? string.Empty,
                    entity.Date,
                    entity.DeliveryDate,
                    entity.DeliveryStartTime,
                    entity.DeliveryEndTime,
                    BusinessName = entity.BusinessName ?? string.Empty,
                    Address = entity.Address ?? string.Empty,
                    Floor = entity.Floor ?? string.Empty,
                    Apartment = entity.Apartment ?? string.Empty,
                    Phone = entity.Phone ?? string.Empty,
                    Observation = entity.Observation ?? string.Empty,
                    entity.GrossPrice,
                    entity.Discounts,
                    entity.TotalPrice,
                    entity.ShippingPrice,
                    TaxStatusId = entity.TaxStatus?.Id,
                    OriginSaleId = entity.OriginSale?.Id,
                    PdfInvoiceId = entity.PdfInvoice?.Id,
                    CityId = entity.City?.Id,
                    LogisticsProviderId = entity.LogisticsProvider?.Id,
                    SearchString = entity.SearchString ?? string.Empty,
                }, transaction: _session.Transaction, cancellationToken: cancellation));
            await _saleDetailRepository.SyncCollection(entity, entity.SaleDetails, cancellation);

            return await Get(entity.Id, cancellation) ?? throw new Exception($"La venta {entity.Number} no se encontro para actualizar.");
        }

        public async Task UpdateState(long saleId, SaleState saleState, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.Sale
                SET State = @State,
                    UpdateDate = SYSUTCDATETIME(),
                    Version = Version + 1
                WHERE Id = @Id AND IsDeleted = 0;
            ";

            await _session.Connection.ExecuteAsync(new CommandDefinition(query, new { Id = saleId, State = saleState.ToString() }, transaction: _session.Transaction, cancellationToken: cancellation));
        }

        public async Task AddLocation(Sale entity, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.Sale
                SET 
                    Latitude = @Latitude, 
                    Longitude = @Longitude, 
                    Address = @Address,
                    Observation = @Observation,
                    CityId = @CityId, 
                    UpdateDate = SYSUTCDATETIME(),
                    Version = Version + 1
                WHERE Id = @Id AND IsDeleted = 0;
            ";
            await _session.Connection.ExecuteAsync(new CommandDefinition(query, new { entity.Id, entity.Longitude, entity.Latitude, entity.Address, entity.Observation, CityId = entity.City.Id }, transaction: _session.Transaction, cancellationToken: cancellation));
        }

        public async Task UpdateIsPrinted(long saleId, bool isPrinted, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.Sale
                SET 
                    IsPrinted = @IsPrinted,
                    UpdateDate = SYSUTCDATETIME(),
                    Version = Version + 1
                WHERE Id = @Id AND IsDeleted = 0;
            ";
            await _session.Connection.ExecuteAsync(new CommandDefinition(query, new { Id = saleId, IsPrinted = isPrinted }, transaction: _session.Transaction, cancellationToken: cancellation));
        }

        public async Task<Sale> Map(dynamic rowDapper, CancellationToken cancellation = default)
        {
            var sale = new Sale();
            sale.Id = rowDapper.Id;
            sale.SearchString = rowDapper.SearchString;
            sale.CreationDate = rowDapper.CreationDate;
            sale.RemoveDate = rowDapper.RemoveDate;
            sale.UpdateDate = rowDapper.UpdateDate;
            sale.Version = rowDapper.Version;
            sale.Guid = rowDapper.Guid;
            sale.IsDeleted = rowDapper.IsDeleted;
            sale.IsPrinted = rowDapper.IsPrinted;
            sale.IsReverseLogistics = rowDapper.IsReverseLogistics;
            sale.Identifier = rowDapper.Identifier;
            sale.Number = rowDapper.Number;
            sale.ArcaNumber = rowDapper.ArcaNumber;
            sale.Dni = rowDapper.Dni;
            sale.Cuit = rowDapper.Cuit;
            sale.Date = rowDapper.Date;
            sale.DeliveryDate = rowDapper.DeliveryDate;
            sale.DeliveryStartTime = rowDapper.DeliveryStartTime;
            sale.DeliveryEndTime = rowDapper.DeliveryEndTime;
            sale.BusinessName = rowDapper.BusinessName;
            sale.Address = rowDapper.Address;
            sale.Floor = rowDapper.Floor;
            sale.Apartment = rowDapper.Apartment;
            sale.Phone = rowDapper.Phone;
            sale.Longitude = rowDapper.Longitude ?? 0;
            sale.Latitude = rowDapper.Latitude ?? 0;
            sale.Observation = rowDapper.Observation;
            sale.GrossPrice = rowDapper.GrossPrice;
            sale.Discounts = rowDapper.Discounts;
            sale.TotalPrice = rowDapper.TotalPrice;
            sale.ShippingPrice = rowDapper.ShippingPrice;
            sale.OriginSale = await _originSaleRepository.Get((long)(rowDapper.OriginSaleId ?? 0), cancellation);
            sale.LogisticsProvider = await _logisticsProviderRepository.Get((long) (rowDapper.LogisticsProviderId ?? 0), cancellation);
            sale.City = await _cityRepository.Get((long)(rowDapper.CityId ?? 0), cancellation);
            sale.State = Enum.Parse<SaleState>(rowDapper.State);
            sale.SaleDetails = await _saleDetailRepository.Get(sale, cancellation);
            sale.TaxStatus = await _taxStatusRepository.Get((long)(rowDapper.TaxStatusId ?? 0), cancellation);
            sale.PdfInvoice = await _storedFileRepository.Get((long)(rowDapper.PdfInvoiceId ?? 0), cancellation);

            return sale;
        }
    }
}
