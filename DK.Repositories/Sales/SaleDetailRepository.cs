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
        private VariantRepository _variantRepository; 
        private ProductColorRepository _productColorRepository;
        private StockRepository _stockRepository;

        public SaleDetailRepository(ISession session, ProductRepository productRepository, VariantRepository variantRepository, ProductColorRepository productColorRepository, StockRepository stockRepository)
        {
            _session = session;
            _productRepository = productRepository;
            _variantRepository = variantRepository;
            _productColorRepository = productColorRepository;
            _stockRepository = stockRepository;
        }

        public async override Task<SaleDetail> Create(Sale parent, SaleDetail entity, CancellationToken cancellation = default)
        {
            const string sql = @"
                INSERT INTO dbo.SaleDetail (SaleId, ProductId, VariantId, ProductColorId, StockId, Count, Price, IsExtra, SearchString)
                OUTPUT INSERTED.*
                VALUES (@SaleId, @ProductId, @VariantId, @ProductColorId, @StockId, @Count, @Price, @IsExtra, @SearchString);";

            entity.SearchString = entity.ToString();
            var rowDapper = await _session.Connection.QuerySingleAsync(
                new CommandDefinition(sql, new { SaleId = parent.Id, ProductId = entity.Product.Id, VariantId = entity.Variant.Id, ProductColorId = entity.Color.Id, StockId = entity.Stock?.Id, entity.Count, entity.Price, entity.IsExtra, entity.SearchString }, _session.Transaction, cancellationToken: cancellation));

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
                entity.Variant?.Id != persited.Variant?.Id ||
                entity.Color?.Id != persited.Color?.Id ||
                entity.Count != persited.Count ||
                entity.Price != persited.Price ||
                entity.IsExtra != persited.IsExtra ||
                entity.Stock?.Id != persited.Stock?.Id;
        }

        public async override Task<SaleDetail> Update(Sale parent, SaleDetail entity, CancellationToken cancellation = default)
        {
            const string sql = @"
                UPDATE dbo.SaleDetail
                   SET ProductId        = @ProductId,
                       VariantId        = @VariantId,
                       ProductColorId   = @ProductColorId,
                       StockId          = @StockId,
                       Count            = @Count, 
                       Price            = @Price,
                       IsExtra          = @IsExtra,
                       SearchString     = @SearchString,
                       UpdateDate       = SYSUTCDATETIME(),
                       Version          = Version + 1
                OUTPUT INSERTED.*
                 WHERE SaleId = @SaleId AND Id = @Id AND IsDeleted = 0;";

            entity.SearchString = entity.ToString();
            var rowDapper = await _session.Connection.QuerySingleOrDefaultAsync(
                new CommandDefinition(sql, new { SaleId = parent.Id, entity.Id, ProductId = entity.Product?.Id, VariantId = entity.Variant?.Id, ProductColorId = entity.Color?.Id, StockId = entity.Stock?.Id, entity.Count, entity.Price, entity.IsExtra, entity.SearchString }, _session.Transaction, cancellationToken: cancellation));

            if (rowDapper is null)
                throw new KeyNotFoundException($"El detalle {entity.Product.Name}-{entity.Variant.Name}-{entity.Color.Name} no se encontro para actualizar.");


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
                throw new KeyNotFoundException($"El detalle {saleDetail.Product.Name}-{saleDetail.Variant.Name}-{saleDetail.Color.Name} no se encontro para actualizar.");
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
                throw new KeyNotFoundException($"El detalle {saleDetail.Product.Name}-{saleDetail.Variant.Name}-{saleDetail.Color.Name} no se encontro para actualizar.");
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
                detail.Variant = await _variantRepository.Get(detail.Product, (long)rowDapper.VariantId, cancellation);
            if (detail.Variant != null)
                detail.Color = await _productColorRepository.Get(detail.Variant, (long)rowDapper.ProductColorId, cancellation);
            detail.Count = rowDapper.Count;
            detail.Price = rowDapper.Price;
            detail.IsExtra = rowDapper.IsExtra;

            if (rowDapper.StockId != null)
                detail.Stock = await _stockRepository.Get((long)rowDapper.StockId, cancellation);

            return detail;
        }
    }
}
