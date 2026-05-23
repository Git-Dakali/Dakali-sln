using Dakali.Interface.Connection;
using Dapper;
using DK.Domain.RoadMaps;
using DK.Domain.Sales;
using DK.Repositories.Base;
using DK.Repositories.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace DK.Repositories.RoadMaps
{
    public class RoadMapSaleRepository : RepositoryReferenceEntity<RoadMap, RoadMapSale>
    {
        private readonly ISession _session;
        private SaleRepository _saleRepository;

        public RoadMapSaleRepository(ISession session, SaleRepository saleRepository)
        {
            _session = session;
            _saleRepository = saleRepository;
        }

        public async override Task<RoadMapSale> Create(RoadMap parent, RoadMapSale entity, CancellationToken cancellation = default)
        {
            var query = @"
                INSERT INTO dbo.RoadMapSale(RoadMapId, SaleId, SortOrder)
                OUTPUT INSERTED.*
                VALUES(@RoadMapId, @SaleId, @SortOrder) ";

            var rowDapper = await _session.Connection.QuerySingleAsync(new CommandDefinition(query, new { RoadMapId = parent.Id, SaleId = entity.Sale.Id, entity.SortOrder }, _session.Transaction, cancellationToken: cancellation));

            return await Map(rowDapper);
        }

        public async override Task Delete(RoadMap parent, RoadMapSale entity, CancellationToken cancellation = default)
        {
            var query = @"DELETE dbo.RoadMapSale WHERE RoadMapId = @RoadMapId AND SaleId = @SaleId;";

            await _session.Connection.ExecuteAsync(new CommandDefinition(query, new { RoadMapId = parent.Id, SaleId = entity.Sale.Id }, transaction: _session.Transaction, cancellationToken: cancellation));
        }

        public async override Task Delete(RoadMap parent, IEnumerable<RoadMapSale> entities, CancellationToken cancellation = default)
        {
            var query = @"DELETE dbo.RoadMapSale WHERE RoadMapId = @RoadMapId;";

            await _session.Connection.ExecuteAsync(new CommandDefinition(query, new { RoadMapId = parent.Id }, transaction: _session.Transaction, cancellationToken: cancellation));
        }

        public async override Task<RoadMapSale> Get(RoadMap parent, long id, CancellationToken cancellation = default)
        {
            throw new NotImplementedException();
        }

        public async Task<RoadMapSale> Get(RoadMap parent, Sale sale, CancellationToken cancellation = default)
        {
            var sql = @"SELECT * FROM RoadMapSale WHERE RoadMapId = @RoadMapId AND SaleId = @SaleId;";

            var rowDapper = await _session.Connection.QuerySingleOrDefaultAsync(new CommandDefinition(sql, new { RoadMapId = parent.Id, SaleId = sale.Id }, _session.Transaction, cancellationToken: cancellation));

            if (rowDapper is null)
                return null;
            
            return await Map(rowDapper);
        }

        public async override Task<IEnumerable<RoadMapSale>> Get(RoadMap parent, CancellationToken cancellation = default)
        {
            var sql = @"SELECT * FROM RoadMapSale WHERE RoadMapId = @RoadMapId ORDER BY SortOrder, Id;";

            var rowsDapper = await _session.Connection.QueryAsync(new CommandDefinition(sql, new { RoadMapId = parent.Id }, _session.Transaction, cancellationToken: cancellation));

            if(rowsDapper is null)
                return Enumerable.Empty<RoadMapSale>();

            var sales = new List<RoadMapSale>();
            foreach(var row in rowsDapper)
                sales.Add(await Map(row));

            return sales;
        }

        public async override Task<bool> HasChanges(RoadMapSale entity, RoadMapSale persited)
        {
            return entity.Id != persited.Id ||
                entity.Sale.Id != persited.Sale.Id ||
                entity.SortOrder != persited.SortOrder;
        }

        public async override Task<RoadMapSale> Update(RoadMap parent, RoadMapSale entity, CancellationToken cancellation = default)
        {
            const string sql = @"
                UPDATE dbo.RoadMapSale
                   SET SortOrder   = @SortOrder
                OUTPUT INSERTED.*
                 WHERE RoadMapId = @RoadMapId AND SaleId = @SaleId;";

            var rowDapper = await _session.Connection.QuerySingleOrDefaultAsync(new CommandDefinition(sql, new { RoadMapId = parent.Id, SaleId = entity.Sale.Id, entity.SortOrder }, _session.Transaction, cancellationToken: cancellation));

            if(rowDapper is null)
                throw new Exception($"El detalle con la venta {entity.Sale.Number} no se encontro para actualizar.");

            return await Map(rowDapper);
        }

        public async Task<RoadMapSale> Map(dynamic rowDapper, CancellationToken cancellation = default)
        { 
            var roadMapSale = new RoadMapSale();
            roadMapSale.Id = rowDapper.Id;
            roadMapSale.SortOrder = rowDapper.SortOrder;
            roadMapSale.Sale = await _saleRepository.Get((long)rowDapper.SaleId, cancellation);

            return roadMapSale;
        }
    }
}
