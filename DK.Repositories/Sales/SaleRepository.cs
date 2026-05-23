using Dakali.Domine;
using Dakali.Interface.Connection;
using Dapper;
using DK.Domain.Sales;
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
        private SaleDetailRepository _saleDetailRepository;

        public SaleRepository(ISession session, TaxStatusRepository taxStatusRepository, OriginSaleRepository originSaleRepository, StoredFileRepository storedFileRepository, CityRepository cityRepository, SaleDetailRepository saleDetailRepository)
        {
            _session = session;
            _taxStatusRepository = taxStatusRepository;
            _originSaleRepository = originSaleRepository;
            _storedFileRepository = storedFileRepository;
            _cityRepository = cityRepository;
            _saleDetailRepository = saleDetailRepository;
        }

        public async Task<Sale> Create(Sale entity, CancellationToken cancellation = default)
        {
            var query = @"
            INSERT INTO dbo.Sale (Identifier, ArcaNumber, Dni, Cuit, Date, DeliveryDate, DeliveryStartTime, DeliveryEndTime, BusinessName, Address, Floor, Apartment, Phone, Observation, GrossPrice, Discounts, TotalPrice, ShippingPrice, TaxStatusId, OriginSaleId, PdfInvoiceId, CityId, State, SearchString)
            OUTPUT INSERTED.*
            VALUES (@Identifier, @ArcaNumber, @Dni, @Cuit, @Date, @DeliveryDate, @DeliveryStartTime, @DeliveryEndTime, @BusinessName, @Address, @Floor, @Apartment, @Phone, @Observation, @GrossPrice, @Discounts, @TotalPrice, @ShippingPrice, @TaxStatusId, @OriginSaleId, @PdfInvoiceId, @CityId, @State, @SearchString);";
            entity.State = SaleState.Creado;
            entity.SearchString = entity.ToString();
            var rowDapper = await _session.Connection.QuerySingleAsync(new CommandDefinition(query, 
                new {
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

            dynamic filter = new ExpandoObject();

            if (saleFilter.Id != null)
            {
                query += " AND Id = @Id";
                queryCount += " AND Id = @Id";

                filter.Id = saleFilter.Id;
            }

            if (!string.IsNullOrWhiteSpace(saleFilter.SearchString))
            {
                query += " AND CONTAINS(SearchString, @SearchString)";
                queryCount += " AND CONTAINS(SearchString, @SearchString)";

                var values = (saleFilter.SearchString ?? string.Empty).Split(" ").Select(value => value.Trim()).Where(value => !string.IsNullOrWhiteSpace(value)).Select(value => $"\"{value}*\"");

                filter.SearchString = $"({string.Join(" AND ", values)})";
            }

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
                    UpdateDate = SYSUTCDATETIME(),
                    Version = Version + 1
                WHERE Id = @Id AND IsDeleted = 0;
            ";
            await _session.Connection.ExecuteAsync(new CommandDefinition(query, new { entity.Id, entity.Longitude, entity.Latitude }, transaction: _session.Transaction, cancellationToken: cancellation));
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
            sale.OriginSale = await _originSaleRepository.Get((long)rowDapper.OriginSaleId, cancellation);
            sale.City = await _cityRepository.Get((long)rowDapper.CityId, cancellation);
            sale.State = Enum.Parse<SaleState>(rowDapper.State);
            sale.SaleDetails = await _saleDetailRepository.Get(sale, cancellation);

            if (rowDapper.TaxStatusId != null)
                sale.TaxStatus = await _taxStatusRepository.Get((long)rowDapper.TaxStatusId, cancellation);
            if (rowDapper.PdfInvoiceId != null)
                sale.PdfInvoice = await _storedFileRepository.Get((long)rowDapper.PdfInvoiceId, cancellation);

            return sale;
        }
    }
}
