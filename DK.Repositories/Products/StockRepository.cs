using Dakali.Interface.Connection;
using Dapper;
using DK.Domain.Locations;
using DK.Domain.Products;
using DK.Repositories.Interface.Base;
using DK.Repositories.Locations;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Repositories.Products
{
    public class StockRepository : IRepository<Stock>
    {
        private ISession _session;
        private ProductRepository _productRepository;
        private ProductSkuRepository _productSkuRepository;
        private LocationRepository _locationRepository;

        public StockRepository(ISession session, ProductRepository productRepository, VariantRepository variantRepository, ProductSkuRepository productSkuRepository, LocationRepository locationRepository) 
        {
            _session = session;
            _productRepository = productRepository;
            _productSkuRepository = productSkuRepository;
            _locationRepository = locationRepository;
        }

        public async Task<Stock> Create(Stock entity, CancellationToken cancellation = default)
        {
            var query = @"
            INSERT INTO dbo.Stock (ProductSkuId, Physical, Reserved, Transit, Free, Minimum, Maximum, LocationId, SearchString)
            OUTPUT INSERTED.*
            VALUES (@ProductSkuId, @Physical, 0, 0, @Physical, 0, 0, @LocationId, @SearchString);";

            entity.SearchString = entity.ToString();
            var rowDapper = await _session.Connection.QuerySingleAsync(new CommandDefinition(query, 
                new { 
                    ProductSkuId = entity.ProductSku.Id,
                    LocationId = entity.Location.Id, 
                    entity.Physical, 
                    entity.SearchString
                }, transaction: _session.Transaction, cancellationToken: cancellation));

            
            return await Map(rowDapper, cancellation);
        }

        public async Task Delete(Stock entity, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.Stock
                   SET IsDeleted = 1,
                       RemoveDate = SYSUTCDATETIME(),
                       UpdateDate = SYSUTCDATETIME(),
                       Version = Version + 1
                 WHERE Id = @id AND IsDeleted = 0;";

            await _session.Connection.ExecuteAsync(new CommandDefinition(query, entity, transaction: _session.Transaction, cancellationToken: cancellation));
        }

        public async Task Delete(Location location, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.Stock
                   SET IsDeleted = 1,
                       RemoveDate = SYSUTCDATETIME(),
                       UpdateDate = SYSUTCDATETIME(),
                       Version = Version + 1
                 WHERE LocationId = @LocationId AND IsDeleted = 0;";

            await _session.Connection.ExecuteAsync(new CommandDefinition(query, new { LocationId = location.Id }, transaction: _session.Transaction, cancellationToken: cancellation));
        }

        public async Task<Stock> Get(long id, CancellationToken cancellation = default)
        {
            var query = @"
                select *
                from dbo.Stock 
                where IsDeleted = 0 AND Id = @Id";
            var rowDapper = await _session.Connection.QuerySingleOrDefaultAsync(new CommandDefinition(query, new { Id = id }, transaction: _session.Transaction, cancellationToken: cancellation));

            return await Map(rowDapper, cancellation);
        }

        public async Task<List<Stock>> Get(Location location, CancellationToken cancellation = default)
        {
            var query = @"
                select *
                from dbo.Stock 
                where IsDeleted = 0 AND LocationId = @LocationId";
            var rowsDapper = await _session.Connection.QueryAsync(new CommandDefinition(query, new { LocationId = location.Id }, transaction: _session.Transaction, cancellationToken: cancellation));

            var list = new List<Stock>();

            foreach (var rowDapper in rowsDapper)
                list.Add(await Map(rowDapper, cancellation));

            return list;
        }

        public async Task<Stock> Get(ProductSku productSku, Location location, CancellationToken cancellation = default)
        {
            var query = @"
                select *
                from dbo.Stock 
                where IsDeleted = 0 AND ProductSkuId = @ProductSkuId AND LocationId = @LocationId";
            var rowDapper = await _session.Connection.QuerySingleOrDefaultAsync(new CommandDefinition(query, new { ProductSkuId = productSku.Id, LocationId = location.Id }, transaction: _session.Transaction, cancellationToken: cancellation));

            return await Map(rowDapper, cancellation);
        }

        public async Task<Stock> Get(LocationState state, ProductSku productSku, long freeCount, CancellationToken cancellation = default)
        {
            var query = @"
                select top 1 *
                from dbo.Stock s
                where IsDeleted = 0 
                    AND ProductSkuId = @ProductSkuId 
                    AND Free >= @Count
                    AND exists(select top 1 1 from dbo.Location l where l.LocationStateId = @LocationStateId)
            ";
            var rowDapper = await _session.Connection.QueryFirstOrDefaultAsync(new CommandDefinition(query, new { ProductSkuId = productSku.Id, Count = freeCount, LocationStateId = state.Id }, transaction: _session.Transaction, cancellationToken: cancellation));

            return await Map(rowDapper, cancellation);
        }

        public async Task<Stock> Reserved(Stock stock, long count, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.Stock
                SET 
                    Free = Free - @Count,
                    Reserved = Reserved + @Count,
                    UpdateDate = SYSUTCDATETIME(),
                    Version = Version + 1
                WHERE Id = @Id AND IsDeleted = 0;
            ";
            var rowDapper = await _session.Connection.ExecuteAsync(new CommandDefinition(query, new { stock.Id, Count = count }, transaction: _session.Transaction, cancellationToken: cancellation));

            return await Get(stock.Id, cancellation);
        }

        public async Task<Stock> Commit(Stock stock, long count, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.Stock
                SET 
                    Physical = Physical - @Count,
                    Reserved = Reserved - @Count,
                    UpdateDate = SYSUTCDATETIME(),
                    Version = Version + 1
                WHERE Id = @Id AND IsDeleted = 0;
            ";
            await _session.Connection.ExecuteAsync(new CommandDefinition(query, new { stock.Id, Count = count }, transaction: _session.Transaction, cancellationToken: cancellation));

            return await Get(stock.Id, cancellation);
        }

        public async Task<Stock> CancelReserved(Stock stock, long count, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.Stock
                SET 
                    Free = Free + @Count,
                    Reserved = Reserved - @Count,
                    UpdateDate = SYSUTCDATETIME(),
                    Version = Version + 1
                WHERE Id = @Id AND IsDeleted = 0;
            ";
            await _session.Connection.ExecuteAsync(new CommandDefinition(query, new { stock.Id, Count = count }, transaction: _session.Transaction, cancellationToken: cancellation));

            return await Get(stock.Id, cancellation);
        }

        public async Task<IEnumerable<Stock>> GetAll(CancellationToken cancellation = default)
        {
            var query = @"
                select *
                from dbo.Stock 
                where IsDeleted = 0 ";
            var rowsDapper = await _session.Connection.QueryAsync(new CommandDefinition(query, new {}, transaction: _session.Transaction, cancellationToken: cancellation));

            if (rowsDapper == null)
                return Enumerable.Empty<Stock>();

            var stocks = new List<Stock>();

            foreach (var rowDapper in rowsDapper)
                stocks.Add(await Map(rowDapper, cancellation));

            return stocks;
        }

        public async Task<IEnumerable<Stock>> GetAll(string searchString, CancellationToken cancellation = default)
        {
            var query = @"
                select *
                from dbo.Stock 
                where IsDeleted = 0";

            if (!string.IsNullOrWhiteSpace(searchString))
                query += " AND CONTAINS(SearchString, @SearchString)";

            var values = (searchString ?? string.Empty).Split(" ").Select(value => value.Trim()).Where(value => !string.IsNullOrWhiteSpace(value)).Select(value => $"\"{value}*\"");

            var rowsDapper = await _session.Connection.QueryAsync(new CommandDefinition(query, new { SearchString = $"({string.Join(" AND ", values)})" }, transaction: _session.Transaction, cancellationToken: cancellation));

            if (rowsDapper == null)
                return Enumerable.Empty<Stock>();

            var stocks = new List<Stock>();

            foreach (var rowDapper in rowsDapper)
                stocks.Add(await Map(rowDapper, cancellation));

            return stocks;
        }

        public async Task<Stock> Update(Stock entity, CancellationToken cancellation = default)
        {
            throw new System.NotImplementedException("No se puede actualizar el Stock");
        }

        public async Task<Stock> UpdatePhysical(Stock entity, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.Stock
                   SET  UpdateDate      = SYSUTCDATETIME(),
                        Version         = Version + 1,
                        Physical        = @Physical,
                        Free            = @Physical - Reserved
                 WHERE Id = @Id AND IsDeleted = 0;";

            await _session.Connection.ExecuteAsync(new CommandDefinition(query, new { entity.Id, entity.Physical}, transaction: _session.Transaction, cancellationToken: cancellation));

            return await Get(entity.Id, cancellation);
        }


        public async Task StockEntry(Stock entity, int amount, CancellationToken cancellationToken = default)
        {
            var query = @"
                UPDATE dbo.Stock
                SET 
                    Physical = Physical + @Amount,
                    Free = Free + @Amount,
                    UpdateDate = SYSUTCDATETIME(),
                    Version = Version + 1
                WHERE Id = @Id AND IsDeleted = 0;
            ";

            await _session.Connection.ExecuteAsync(new CommandDefinition(query, new { entity.Id, Amount = amount }, transaction: _session.Transaction, cancellationToken: cancellationToken));
        }

        public async Task<Stock> Map(dynamic rowDapper, CancellationToken cancellation = default)
        {
            if (rowDapper is null)
                return null;

            var newStock = new Stock();
            newStock.Id = rowDapper.Id;
            newStock.SearchString = rowDapper.SearchString;
            newStock.CreationDate = rowDapper.CreationDate;
            newStock.UpdateDate = rowDapper.UpdateDate;
            newStock.RemoveDate = rowDapper.RemoveDate;
            newStock.IsDeleted = rowDapper.IsDeleted;
            newStock.Guid = rowDapper.Guid;
            newStock.Physical = rowDapper.Physical;
            newStock.Reserved = rowDapper.Reserved;
            newStock.Transit = rowDapper.Transit;
            newStock.Free = rowDapper.Free;
            newStock.Minimum = rowDapper.Minimum;
            newStock.Maximum = rowDapper.Maximum;
            newStock.Location = await _locationRepository.Get(rowDapper.LocationId, cancellation);
            newStock.ProductSku = await _productSkuRepository.Get(((long)rowDapper.ProductSkuId), cancellation);

            return newStock;
        }
    }
}
