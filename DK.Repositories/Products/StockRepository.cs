using Dakali.Domine.Base;
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
        private VariantRepository _variantRepository;
        private ProductColorRepository _colorRepository;
        private LocationRepository _locationRepository;

        public StockRepository(ISession session, ProductRepository productRepository, VariantRepository variantRepository, ProductColorRepository colorRepository, LocationRepository locationRepository) 
        {
            _session = session;
            _productRepository = productRepository;
            _variantRepository = variantRepository;
            _colorRepository = colorRepository;
            _locationRepository = locationRepository;
        }

        public async Task<Stock> Create(Stock entity, CancellationToken cancellation = default)
        {
            var query = @"
            INSERT INTO dbo.Stock (ProductId, VariantId, ProductColorId, Physical, Reserved, Transit, Free, Minimum, Maximum, LocationId, SearchString)
            OUTPUT INSERTED.*
            VALUES (@ProductId, @VariantId, @ProductColorId, @Physical, @Reserved, @Transit, @Free, @Minimum, @Maximum, @LocationId, @SearchString);";

            entity.SearchString = entity.ToString();
            var stock = await _session.Connection.QuerySingleAsync<Stock>(query, 
                new { 
                    ProductId = entity.Product.Id, 
                    VariantId = entity.Variant.Id,
                    ProductColorId = entity.Color.Id,
                    LocationId = entity.Location.Id, 
                    entity.Physical, 
                    entity.Reserved, 
                    entity.Transit,
                    entity.Free, 
                    entity.Minimum, 
                    entity.Maximum, 
                    entity.SearchString
                }, transaction: _session.Transaction);

            if (stock != null)
            {
                stock.Product = await _productRepository.Get(entity.Product.Id, cancellation);
                stock.Variant = await _variantRepository.Get(entity.Product, entity.Variant.Id, cancellation);
                stock.Color = await _colorRepository.Get(entity.Variant, entity.Color.Id, cancellation);
                stock.Location = await _locationRepository.Get(entity.Location.Id, cancellation);
            }

            return stock;
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

            await _session.Connection.ExecuteAsync(query, entity, transaction: _session.Transaction);
        }

        public async Task<Stock> Get(long id, CancellationToken cancellation = default)
        {
            var query = @"
                select *
                from dbo.Stock 
                where IsDeleted = 0 AND Id = @Id";
            var rowDapper = await _session.Connection.QuerySingleOrDefaultAsync<dynamic>(query, new { Id = id }, transaction: _session.Transaction);

            if (rowDapper == null)
                return null;

            return await Map(rowDapper, cancellation);
        }

        public async Task<Stock> Get(Product product, Variant variant, ProductColor color, Location location, CancellationToken cancellation = default)
        {
            var query = @"
                select *
                from dbo.Stock 
                where IsDeleted = 0 AND ProductId = @ProductId AND VariantId = @VariantId AND ProductColorId = @ProductColorId AND LocationId = @LocationId";
            var rowDapper = await _session.Connection.QuerySingleOrDefaultAsync<dynamic>(query, new { ProductId = product.Id, VariantId = variant.Id, ProductColorId = color.Id, LocationId = location.Id }, transaction: _session.Transaction);

            if (rowDapper == null)
                return null;

            return await Map(rowDapper, cancellation);
        }

        public async Task<Stock> Get(LocationState state, Product product, Variant variant, ProductColor color, long freeCount, CancellationToken cancellation = default)
        {
            var query = @"
                select top 1 *
                from dbo.Stock s
                where IsDeleted = 0 
                    AND ProductId = @ProductId 
                    AND VariantId = @VariantId 
                    AND ProductColorId = @ProductColorId 
                    AND Free >= @Count
                    AND exists(select top 1 1 from dbo.Location l where l.LocationStateId = @LocationStateId)
            ";
            var rowDapper = await _session.Connection.QueryFirstOrDefaultAsync(query, new { ProductId = product.Id, VariantId = variant.Id, ProductColorId = color.Id, Count = freeCount, LocationStateId = state.Id }, transaction: _session.Transaction);

            if (rowDapper == null)
                return null;

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
            var rowDapper = await _session.Connection.QueryFirstOrDefaultAsync(query, new { stock.Id, Count = count }, transaction: _session.Transaction);

            if (rowDapper == null)
                return null;

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
            await _session.Connection.ExecuteAsync(query, new { stock.Id, Count = count }, transaction: _session.Transaction);


            return await Get(stock.Id, cancellation);
        }

        public async Task<IEnumerable<Stock>> GetAll(CancellationToken cancellation = default)
        {
            var query = @"
                select *
                from dbo.Stock 
                where IsDeleted = 0 ";
            var rowsDapper = await _session.Connection.QueryAsync(query, new {}, transaction: _session.Transaction);

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

            var rowsDapper = await _session.Connection.QueryAsync(query, new { SearchString = $"({string.Join(" AND ", values)})" }, transaction: _session.Transaction);

            if (rowsDapper == null)
                return Enumerable.Empty<Stock>();

            var stocks = new List<Stock>();

            foreach (var rowDapper in rowsDapper)
                stocks.Add(await Map(rowDapper, cancellation));

            return stocks;
        }

        public async Task<Stock> Update(Stock entity, CancellationToken cancellation = default)
        {
            throw new System.Exception("La actualizacion del Stock esta bloqueado, revise con un Administrador.");
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

            await _session.Connection.ExecuteAsync(query, new { entity.Id, Amount = amount }, transaction: _session.Transaction);
        }

        public async Task<Stock> Map(dynamic rowDapper, CancellationToken cancellation = default)
        {
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
            newStock.Product = await _productRepository.Get(rowDapper.ProductId, cancellation);
            newStock.Variant = await _variantRepository.Get(newStock.Product, rowDapper.VariantId, cancellation);
            newStock.Color = await _colorRepository.Get(newStock.Variant, rowDapper.ProductColorId, cancellation);

            return newStock;
        }
    }
}
