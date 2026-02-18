using Dakali.Interface.Connection;
using Dapper;
using DK.Domain.Products;
using DK.Domain.Sales;
using DK.Repositories.GeographicLocation;
using DK.Repositories.Interface.Base;
using DK.Repositories.Products;
using System;
using System.Collections.Generic;
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

            entity.SearchString = entity.ToString();
            var rowDapper = await _session.Connection.QuerySingleAsync(query, 
                new { 
                    entity.Identifier, 
                    entity.ArcaNumber,
                    entity.Dni,
                    entity.Cuit,
                    entity.Date, 
                    entity.DeliveryDate, 
                    entity.DeliveryStartTime, 
                    entity.DeliveryEndTime, 
                    entity.BusinessName, 
                    entity.Address, 
                    entity.Floor, 
                    entity.Apartment, 
                    entity.Phone,
                    entity.Observation,
                    entity.GrossPrice,
                    entity.Discounts,
                    entity.TotalPrice,
                    entity.ShippingPrice,
                    TaxStatusId = entity.TaxStatus?.Id,
                    OriginSaleId = entity.OriginSale?.Id,
                    PdfInvoiceId = entity.PdfInvoice?.Id,
                    CityId = entity.City?.Id,
                    State = SaleState.Creado.ToString(),
                    entity.SearchString 
                }, transaction: _session.Transaction);

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
                       RemoveDate = SYSUTCDATETIME(),
                       UpdateDate = SYSUTCDATETIME(),
                       Version = Version + 1
                 WHERE Id = @id AND IsDeleted = 0;";

            await _session.Connection.ExecuteAsync(query, entity, transaction: _session.Transaction);
            await _saleDetailRepository.Delete(entity, entity.SaleDetails, cancellation);
        }

        public async Task<Sale> Get(long id, CancellationToken cancellation = default)
        {
            var query = @"
                select *
                from dbo.Sale 
                where IsDeleted = 0 AND Id = @Id";
            var rowDapper = await _session.Connection.QuerySingleOrDefaultAsync(query, new { Id = id }, transaction: _session.Transaction);

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
            var rowsDapper = await _session.Connection.QueryAsync(query, new { }, transaction: _session.Transaction);

            if (rowsDapper is null)
                return Enumerable.Empty<Sale>();

            var sales = new List<Sale>();

            foreach (var row in rowsDapper)
                sales.Add(await Map(row));

            return sales;
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
            await _session.Connection.QuerySingleAsync<Model>(query, 
                new {
                    entity.Id, entity.Identifier, entity.ArcaNumber, entity.Dni, entity.Cuit, entity.Date, entity.DeliveryDate, entity.DeliveryStartTime, entity.DeliveryEndTime, entity.BusinessName, entity.Address, entity.Floor, entity.Apartment, entity.Phone,
                    entity.Observation, entity.GrossPrice, entity.Discounts, entity.TotalPrice, entity.ShippingPrice, TaxStatusId = entity.TaxStatus?.Id, OriginSaleId = entity.OriginSale?.Id, PdfInvoiceId = entity.PdfInvoice?.Id, CityId = entity.City?.Id, entity.SearchString,
                }, transaction: _session.Transaction);
            await _saleDetailRepository.SyncCollection(entity, entity.SaleDetails, cancellation);

            return await Get(entity.Id, cancellation) ?? throw new Exception($"La venta {entity.Number} no se encontro para actualizar.");
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
