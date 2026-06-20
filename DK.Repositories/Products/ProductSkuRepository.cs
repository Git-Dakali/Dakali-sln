using Dakali.Interface.Connection;
using Dapper;
using DK.Domain.Products;
using DK.Repositories.Base;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Repositories.Products
{
    public class ProductSkuRepository : RepositoryReferenceEntity<Product, ProductSku>
    {
        private readonly ISession _session;
        private ProductColorRepository _productColorRepository;
        private VariantRepository _variantRepository;
        private IServiceProvider _serviceProvider;

        public ProductSkuRepository(ISession session, IServiceProvider serviceProvider, ProductColorRepository productColorRepository, VariantRepository variantRepository)
        {
            _session = session;
            _productColorRepository = productColorRepository;
            _variantRepository = variantRepository;
            _serviceProvider = serviceProvider;
        }

        public async override Task<ProductSku> Create(Product parent, ProductSku entity, CancellationToken cancellation = default)
        {
            const string sql = @"
                INSERT INTO dbo.ProductSku (ProductId, ProductColorId, VariantId, Sku, SearchString)
                OUTPUT INSERTED.*
                VALUES (@ProductId, @ProductColorId, @VariantId, @Sku, @SearchString);";

            entity.SearchString = entity.ToString();
            var rowDapper = await _session.Connection.QuerySingleAsync(new CommandDefinition(sql, new { ProductId = parent.Id, ProductColorId = entity.Color.Id, VariantId = entity.Variant.Id, Sku = entity.Sku.ToUpper(), entity.SearchString }, _session.Transaction, cancellationToken: cancellation));
            return await Map(rowDapper, cancellation);
        }

        public async override Task Delete(Product parent, ProductSku entity, CancellationToken cancellation = default)
        {
            const string sql = @"
                UPDATE dbo.ProductSku
                   SET IsDeleted  = 1,
                       RemoveDate = SYSUTCDATETIME(),
                       UpdateDate = SYSUTCDATETIME(),
                       Version    = Version + 1
                 WHERE ProductId = @ProductId AND Id = @Id AND IsDeleted = 0;";

            await _session.Connection.ExecuteAsync(
                new CommandDefinition(sql, new { ProductId = parent.Id, entity.Id }, _session.Transaction, cancellationToken: cancellation));
        }

        public async override Task Delete(Product parent, IEnumerable<ProductSku> entities, CancellationToken cancellation = default)
        {
            if (entities is null)
                return;

            foreach (var entity in entities)
                await Delete(parent, entity, cancellation);
        }

        public async Task<ProductSku> Get(long id, CancellationToken cancellation = default)
        {
            const string sql = @"
                SELECT *
                  FROM dbo.ProductSku
                 WHERE Id = @Id AND IsDeleted = 0;";

            var rowDapper = await _session.Connection.QuerySingleOrDefaultAsync(
                new CommandDefinition(sql, new { Id = id }, _session.Transaction, cancellationToken: cancellation));

            return await Map(rowDapper, cancellation);
        }

        public async override Task<ProductSku> Get(Product parent, long id, CancellationToken cancellation = default)
        {
            const string sql = @"
                SELECT *
                  FROM dbo.ProductSku
                 WHERE ProductId = @ProductId AND Id = @Id AND IsDeleted = 0;";

            var rowDapper = await _session.Connection.QuerySingleOrDefaultAsync(
                new CommandDefinition(sql, new { ProductId = parent.Id, Id = id }, _session.Transaction, cancellationToken: cancellation));

            return await Map(rowDapper, cancellation);
        }

        public async Task<ProductSku> GetBySku(string sku, CancellationToken cancellation = default)
        {
            const string sql = @"
                SELECT *
                  FROM dbo.ProductSku
                 WHERE Sku = @Sku AND IsDeleted = 0;";

            var rowDapper = await _session.Connection.QuerySingleOrDefaultAsync(
                new CommandDefinition(sql, new { Sku = sku }, _session.Transaction, cancellationToken: cancellation));

            return await Map(rowDapper, cancellation);
        }

        public async override Task<IEnumerable<ProductSku>> Get(Product parent, CancellationToken cancellation = default)
        {
            const string sql = @"
                SELECT *
                  FROM dbo.ProductSku
                 WHERE ProductId = @ProductId AND IsDeleted = 0;";

            var rows = await _session.Connection.QueryAsync(
                new CommandDefinition(sql, new { ProductId = parent.Id }, _session.Transaction, cancellationToken: cancellation));

            var skus = new List<ProductSku>();
            foreach (var row in rows)   
                skus.Add(await Map(row, cancellation));
            return skus;
        }

        public async override Task<bool> HasChanges(ProductSku entity, ProductSku persited)
        {
            return entity.Id != persited.Id ||
                entity.Color.Id != persited.Color.Id ||
                entity.Variant.Id != persited.Variant.Id ||
                entity.Sku.ToUpper() != persited.Sku.ToUpper();
        }

        public async override Task<ProductSku> Update(Product parent, ProductSku entity, CancellationToken cancellation = default)
        {
            const string sql = @"
                UPDATE dbo.ProductSku
                   SET ProductColorId   = @ProductColorId,
                       VariantId        = @VariantId,
                       Sku              = @Sku,
                       SearchString     = @SearchString
                OUTPUT INSERTED.*
                 WHERE Id = @Id AND IsDeleted = 0;";

            entity.SearchString = entity.ToString();
            var rowDapper = await _session.Connection.QuerySingleOrDefaultAsync(
                new CommandDefinition(sql, new { entity.Id, ProductColorId= entity.Color.Id, VariantId= entity.Variant.Id, entity.Sku, entity.SearchString }, _session.Transaction, cancellationToken: cancellation));

            return await Map(rowDapper, cancellation) ?? throw new KeyNotFoundException($"SKU {entity.Sku} no encontrado para actualizar.");
        }

        public async Task<ProductSku> Map(dynamic rowDapper, CancellationToken cancellation = default)
        {
            if (rowDapper is null)
                return null;

            var productRepository = _serviceProvider.GetService<ProductRepository>();

            var productSku = new ProductSku();
            productSku.Id = rowDapper.Id;
            productSku.SearchString = rowDapper.SearchString;
            productSku.CreationDate = rowDapper.CreationDate;
            productSku.RemoveDate = rowDapper.RemoveDate;
            productSku.UpdateDate = rowDapper.UpdateDate;
            productSku.Version = rowDapper.Version;
            productSku.Guid = rowDapper.Guid;
            productSku.IsDeleted = rowDapper.IsDeleted;
            productSku.Sku = rowDapper.Sku;
            productSku.Product = await productRepository?.GetLight(rowDapper.ProductId, cancellation);
            productSku.Color = await _productColorRepository.Get(productSku.Product, rowDapper.ProductColorId, cancellation);
            productSku.Variant = await _variantRepository.Get(productSku.Product, rowDapper.VariantId, cancellation);

            return productSku;
        }
    }
}
