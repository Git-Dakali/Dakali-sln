using Dakali.Interface.Connection;
using Dapper;
using DK.Domain.Products;
using DK.Repositories.Interface.Base;
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
        private ColorRepository _colorRepository;
        private StockStateRepository _stockStateRepository;

        public StockRepository(ISession session, ProductRepository productRepository, VariantRepository variantRepository, ColorRepository colorRepository, StockStateRepository stockStateRepository) 
        {
            _session = session;
            _productRepository = productRepository;
            _variantRepository = variantRepository;
            _colorRepository = colorRepository;
            _stockStateRepository = stockStateRepository;
        }

        public async Task<Stock> Create(Stock entity, CancellationToken cancellation = default)
        {
            var query = @"
            INSERT INTO dbo.Stock (ProductId, VariantId, ColorId, Physical, Reserved, Transit, Free, Minimum, Maximum, StockStateId, SearchString)
            OUTPUT INSERTED.*
            VALUES (@ProductId, @VariantId, @ColorId, @Physical, @Reserved, @Transit, @Free, @Minimum, @Maximum, @StockStateId, @SearchString);";

            entity.SearchString = entity.ToString();
            var stock = await _session.Connection.QuerySingleAsync<Stock>(query, 
                new { 
                    ProductId = entity.Product.Id, 
                    VariantId = entity.Variant.Id, 
                    ColorId = entity.Color.Id, 
                    StockStateId = entity.State.Id, 
                    entity.Physical, 
                    entity.Reserved, 
                    entity.Transit,
                    entity.Free, 
                    entity.Minimum, 
                    entity.Maximum, 
                    entity.State,
                    entity.SearchString
                }, transaction: _session.Transaction);

            if (stock != null)
            {
                stock.Product = await _productRepository.Get(entity.Product.Id, cancellation);
                stock.Variant = await _variantRepository.Get(entity.Product, entity.Variant.Id, cancellation);
                stock.Color = await _colorRepository.Get(entity.Variant, entity.Color.Id, cancellation);
                stock.State = await _stockStateRepository.Get(entity.State.Id, cancellation);
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
            newStock.State = await _stockStateRepository.Get(rowDapper.StockStateId, cancellation);
            newStock.Product = await _productRepository.Get(rowDapper.ProductId, cancellation);
            newStock.Variant = await _variantRepository.Get(newStock.Product, rowDapper.VariantId, cancellation);
            newStock.Color = await _colorRepository.Get(newStock.Variant, rowDapper.ColorId, cancellation);

            return newStock;
        }

        public async Task<IEnumerable<Stock>> GetAll(CancellationToken cancellation = default)
        {
            var query = @"
                select *
                from dbo.Stock 
                where IsDeleted = 0";
            var rowsDapper = await _session.Connection.QueryAsync(query, new { }, transaction: _session.Transaction);

            if (rowsDapper == null)
                return Enumerable.Empty<Stock>();

            var stocks = new List<Stock>();
            foreach (var rowDapper in rowsDapper)
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
                newStock.State = await _stockStateRepository.Get(rowDapper.StockStateId, cancellation);
                newStock.Product = await _productRepository.Get(rowDapper.ProductId, cancellation);
                newStock.Variant = await _variantRepository.Get(newStock.Product, rowDapper.VariantId ?? 0, cancellation);
                newStock.Color = await _colorRepository.Get(newStock.Variant, rowDapper.ColorId ?? 0, cancellation);

                stocks.Add(newStock);
            }
            return stocks;
        }

        public async Task<Stock> Update(Stock entity, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.Stock
                SET 
                    Physical = @Physical,
                    Reserved = @Reserved,
                    Transit = @Transit,
                    Free = @Free,
                    Minimum = @Minimum,
                    Maximum = @Maximum,
                    StockStateId = @StockStateId,
                    ProductId = @ProductId,
                    VariantId = @VariantId,
                    ColorId = @ColorId,
                    SearchString = @SearchString,
                    UpdateDate = SYSUTCDATETIME(),
                    Version = Version + 1
                OUTPUT INSERTED.*
                WHERE Id = @Id AND IsDeleted = 0;
            ";

            entity.SearchString = entity.ToString();
            await _session.Connection.QuerySingleAsync<Model>(query, new { 
                entity.Id, entity.Physical, entity.Reserved, entity.Transit, entity.Free, entity.Minimum, entity.Maximum,
                StockStateId = entity.State.Id, ProductId = entity.Product.Id, VariantId = entity.Variant.Id, ColorId = entity.Color.Id, entity.SearchString
            }, transaction: _session.Transaction);
            
            return await Get(entity.Id, cancellation) ?? throw new KeyNotFoundException($"El Stock {entity.Product.Name}-{entity.Variant.Name}-{entity.Color.Name} no se encontro para actualizar.");
        }
    }
}
