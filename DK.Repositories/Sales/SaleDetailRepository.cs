using Dakali.Interface.Connection;
using Dapper;
using DK.Domain.Products;
using DK.Domain.Sales;
using DK.Repositories.Base;
using DK.Repositories.Products;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Repositories.Sales
{
    public class SaleDetailRepository : RepositoryReferenceEntity<Sale, SaleDetail>
    {
        private readonly ISession _session;
        private ProductRepository _productRepository; 
        private ProductSkuRepository _productSkuRepository;
        private StockRepository _stockRepository;

        public SaleDetailRepository(ISession session, ProductRepository productRepository, ProductSkuRepository productSkuRepository, StockRepository stockRepository)
        {
            _session = session;
            _productRepository = productRepository;
            _stockRepository = stockRepository;
            _productSkuRepository = productSkuRepository;
        }

        public async override Task<SaleDetail> Create(Sale parent, SaleDetail entity, CancellationToken cancellation = default)
        {
            const string sql = @"
                INSERT INTO dbo.SaleDetail (SaleId, ProductId, ProductSkuId, StockId, Count, Price, IsExchangeItem, SearchString)
                OUTPUT INSERTED.*
                VALUES (@SaleId, @ProductId, @ProductSkuId, @StockId, @Count, @Price, @IsExchangeItem, @SearchString);";

            entity.SearchString = entity.ToString();
            var rowDapper = await _session.Connection.QuerySingleAsync(
                new CommandDefinition(sql, new { SaleId = parent.Id, ProductId = entity.Product.Id, ProductSkuId = entity.ProductSku.Id, StockId = entity.Stock?.Id, entity.Count, entity.Price, entity.IsExchangeItem, entity.SearchString }, _session.Transaction, cancellationToken: cancellation));

            if (rowDapper is null)
                return null;

            return await Map(rowDapper, cancellation);
        }

        public async override Task Delete(Sale parent, SaleDetail entity, CancellationToken cancellation = default)
        {
            const string sql = @"
                UPDATE dbo.SaleDetail
                   SET IsDeleted  = 1,
                       RemoveDate = SYSUTCDATETIME(),
                       UpdateDate = SYSUTCDATETIME(),
                       Version    = Version + 1
                 WHERE SaleId = @SaleId AND Id = @Id AND IsDeleted = 0;";

            await _session.Connection.ExecuteAsync(new CommandDefinition(sql, new { SaleId = parent.Id, entity.Id }, _session.Transaction, cancellationToken: cancellation));

            if (entity.Stock != null)
                await _stockRepository.CancelReserved(entity.Stock, entity.Count);
        }

        public async override Task Delete(Sale parent, IEnumerable<SaleDetail> entities, CancellationToken cancellation = default)
        {
            if (entities is null)
                return;

            foreach (var entity in entities)
                await Delete(parent, entity, cancellation);
        }

        public async override Task<SaleDetail> Get(Sale parent, long id, CancellationToken cancellation = default)
        {
            const string sql = @"
                SELECT *
                  FROM dbo.SaleDetail
                 WHERE SaleId = @SaleId AND Id = @Id AND IsDeleted = 0;";

            var rowDapper = await _session.Connection.QuerySingleOrDefaultAsync(
                new CommandDefinition(sql, new { SaleId = parent.Id, Id = id }, _session.Transaction, cancellationToken: cancellation));

            if (rowDapper is null)
                return null;

            return await Map(rowDapper, cancellation);
        }

        public async override Task<IEnumerable<SaleDetail>> Get(Sale parent, CancellationToken cancellation = default)
        {
            const string sql = @"
                SELECT *
                  FROM dbo.SaleDetail
                 WHERE SaleId = @SaleId AND IsDeleted = 0;";

            var rowsDapper = await _session.Connection.QueryAsync(
                new CommandDefinition(sql, new { SaleId = parent.Id }, _session.Transaction, cancellationToken: cancellation));

            if (rowsDapper is null)
                return Enumerable.Empty<SaleDetail>();

            var details = new List<SaleDetail>();
            
            foreach (var rowDapper in rowsDapper)
                details.Add(await Map(rowDapper, cancellation));

            return details;
        }

        public async override Task<bool> HasChanges(SaleDetail entity, SaleDetail persited)
        {
            return entity.Id != persited.Id ||
                entity.Product?.Id != persited.Product?.Id ||
                entity.ProductSku?.Id != persited.ProductSku?.Id ||
                entity.Count != persited.Count ||
                entity.Price != persited.Price ||
                entity.IsExchangeItem != persited.IsExchangeItem ||
                entity.Stock?.Id != persited.Stock?.Id;
        }

        public async override Task<SaleDetail> Update(Sale parent, SaleDetail entity, CancellationToken cancellation = default)
        {
            const string sql = @"
                UPDATE dbo.SaleDetail
                   SET ProductId        = @ProductId,
                       VariantId        = @VariantId,
                       ProductSkuId     = @ProductSkuId,
                       StockId          = @StockId,
                       Count            = @Count, 
                       Price            = @Price,
                       IsExchangeItem   = @IsExchangeItem,
                       SearchString     = @SearchString,
                       UpdateDate       = SYSUTCDATETIME(),
                       Version          = Version + 1
                OUTPUT INSERTED.*
                 WHERE SaleId = @SaleId AND Id = @Id AND IsDeleted = 0;";

            entity.SearchString = entity.ToString();
            var rowDapper = await _session.Connection.QuerySingleOrDefaultAsync(
                new CommandDefinition(sql, new { SaleId = parent.Id, entity.Id, ProductId = entity.Product?.Id, ProductSkuId = entity.ProductSku?.Id, StockId = entity.Stock?.Id, entity.Count, entity.Price, entity.IsExchangeItem, entity.SearchString }, _session.Transaction, cancellationToken: cancellation));

            if (rowDapper is null)
                throw new KeyNotFoundException($"El detalle {entity.Product.Name}-{entity.ProductSku.Variant.Name}-{entity.ProductSku.Color.Name} no se encontro para actualizar.");


            return await Map(rowDapper, cancellation);
        }

        public async Task AssignStock(Sale parent, SaleDetail saleDetail, Stock stock, CancellationToken cancellation = default)
        {
            const string sql = @"
                UPDATE dbo.SaleDetail
                   SET StockId          = @StockId,
                       SearchString     = @SearchString,
                       UpdateDate       = SYSUTCDATETIME(),
                       Version          = Version + 1
                OUTPUT INSERTED.*
                 WHERE SaleId = @SaleId AND Id = @Id AND IsDeleted = 0;";

            saleDetail.SearchString = saleDetail.ToString();
            var rowDapper = await _session.Connection.QuerySingleOrDefaultAsync(
                new CommandDefinition(sql, new { SaleId = parent.Id, saleDetail.Id, StockId = stock.Id, saleDetail.SearchString }, _session.Transaction, cancellationToken: cancellation));

            if (rowDapper is null)
                throw new KeyNotFoundException($"El detalle {saleDetail.Product.Name}-{saleDetail.ProductSku.Variant.Name}-{saleDetail.ProductSku.Color.Name} no se encontro para actualizar.");
        }

        public async Task UnassignStock(Sale parent, SaleDetail saleDetail, CancellationToken cancellation = default)
        {
            const string sql = @"
                UPDATE dbo.SaleDetail
                   SET StockId          = null,
                       SearchString     = @SearchString,
                       UpdateDate       = SYSUTCDATETIME(),
                       Version          = Version + 1
                OUTPUT INSERTED.*
                 WHERE SaleId = @SaleId AND Id = @Id AND IsDeleted = 0;";

            saleDetail.SearchString = saleDetail.ToString();
            var rowDapper = await _session.Connection.QuerySingleOrDefaultAsync(
                new CommandDefinition(sql, new { SaleId = parent.Id, saleDetail.Id, saleDetail.SearchString }, _session.Transaction, cancellationToken: cancellation));

            if (rowDapper is null)
                throw new KeyNotFoundException($"El detalle {saleDetail.Product.Name}-{saleDetail.ProductSku.Variant.Name}-{saleDetail.ProductSku.Color.Name} no se encontro para actualizar.");
        }

        public async Task<SaleDetail> Map(dynamic rowDapper, CancellationToken cancellation = default)
        {
            var detail = new SaleDetail();
            detail.Id = rowDapper.Id;
            detail.SearchString = rowDapper.SearchString;
            detail.CreationDate = rowDapper.CreationDate;
            detail.RemoveDate = rowDapper.RemoveDate;
            detail.UpdateDate = rowDapper.UpdateDate;
            detail.Version = rowDapper.Version;
            detail.Guid = rowDapper.Guid;
            detail.IsDeleted = rowDapper.IsDeleted;
            detail.Product = await _productRepository.Get((long)rowDapper.ProductId, cancellation);
            if (detail.Product != null)
                detail.ProductSku = await _productSkuRepository.Get(detail.Product, (long)rowDapper.ProductSkuId, cancellation);
            detail.Count = rowDapper.Count;
            detail.Price = rowDapper.Price;
            detail.IsExchangeItem = rowDapper.IsExchangeItem;

            if (rowDapper.StockId != null)
                detail.Stock = await _stockRepository.Get((long)rowDapper.StockId, cancellation);

            return detail;
        }
    }
}
