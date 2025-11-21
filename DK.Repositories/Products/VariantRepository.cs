using Dakali.Interface.Connection;
using Dapper;
using DK.Domain.Products;
using DK.Repositories.Base;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Repositories.Products
{
    public class VariantRepository : RepositoryReferenceEntity<Product, Variant>
    {
        private readonly ISession _session;
        private readonly ImageRepository _productImageRepository;
        private readonly AttributeRepository _productAttributeRepository;

        public VariantRepository(ISession session, ImageRepository productImageRepository, AttributeRepository productAttributeRepository)
        {
            _session = session;
            _productImageRepository = productImageRepository;
            _productAttributeRepository = productAttributeRepository;
        }

        public async override Task<Variant> Create(Product parent, Variant entity, CancellationToken cancellation = default)
        {
            const string sql = @"
                INSERT INTO dbo.Variant (ProductId, [Size])
                OUTPUT INSERTED.Id, INSERTED.ProductId, INSERTED.[Size],
                       INSERTED.CreationDate, INSERTED.UpdateDate, INSERTED.RemoveDate, INSERTED.Version, INSERTED.Guid, INSERTED.IsDeleted
                VALUES (@ProductId, @Size);";

            var variant = await _session.Connection.QuerySingleAsync<Variant>(
                new CommandDefinition(sql, new { parent.Id, entity.Size }, _session.Transaction, cancellationToken: cancellation));
            variant.Attributes = (await _productAttributeRepository.SyncCollection(variant, entity.Attributes, cancellation)).ToList();
            variant.Images = (await _productImageRepository.SyncCollection(variant, entity.Images, cancellation)).ToList();
            return variant;
        }

        public async override Task Delete(Product parent, Variant entity, CancellationToken cancellation = default)
        {
            await _productImageRepository.Delete(entity, entity.Images, cancellation);

            const string sql = @"
                UPDATE dbo.Variant
                   SET IsDeleted  = 1,
                       RemoveDate = SYSUTCDATETIME(),
                       UpdateDate = SYSUTCDATETIME(),
                       Version    = Version + 1
                 WHERE ProductId = @ProductId AND Id = @Id AND IsDeleted = 0;";

            await _session.Connection.ExecuteAsync(
                new CommandDefinition(sql, new { ProductId = parent.Id, entity.Id }, _session.Transaction, cancellationToken: cancellation));

            await _productAttributeRepository.Delete(entity, entity.Attributes);
            await _productImageRepository.Delete(entity, entity.Images);
        }

        public async override Task Delete(Product parent, IEnumerable<Variant> entities, CancellationToken cancellation = default)
        {
            if (entities is null)
                return;

            foreach (var entity in entities)
                await Delete(parent, entity, cancellation);
        }

        public async override Task<Variant> Get(Product parent, long id, CancellationToken cancellation = default)
        {
            const string sql = @"
                SELECT Id, ProductId, [Size],
                       CreationDate, UpdateDate, RemoveDate, Version, Guid, IsDeleted
                  FROM dbo.Variant
                 WHERE ProductId = @ProductId AND Id = @Id AND IsDeleted = 0;";

            var variant = await _session.Connection.QuerySingleOrDefaultAsync<Variant>(
                new CommandDefinition(sql, new { ProductId = parent.Id, Id = id }, _session.Transaction, cancellationToken: cancellation));

            if (variant != null)
            {
                variant.Images = (await _productImageRepository.Get(variant)).ToList();
                variant.Attributes = (await _productAttributeRepository.Get(variant)).ToList();
            }

            return variant;
        }

        public async override Task<IEnumerable<Variant>> Get(Product parent, CancellationToken cancellation = default)
        {
            const string sql = @"
                SELECT Id, ProductId, [Size],
                       CreationDate, UpdateDate, RemoveDate, Version, Guid, IsDeleted
                  FROM dbo.Variant
                 WHERE ProductId = @ProductId AND IsDeleted = 0;";

            var variants = await _session.Connection.QueryAsync<Variant>(
                new CommandDefinition(sql, new { ProductId = parent.Id }, _session.Transaction, cancellationToken: cancellation));

            foreach (var variant in variants)
            {
                variant.Images = (await _productImageRepository.Get(variant)).ToList();
                variant.Attributes = (await _productAttributeRepository.Get(variant)).ToList();
            }
            return variants;
        }

        public override bool HasChanges(Variant entity, Variant persited)
        {
            return entity.Id != persited.Id ||
                entity.ColorsHex != persited.ColorsHex ||
                entity.Cost != persited.Cost;
        }

        public async override Task<Variant> Update(Product parent, Variant entity, CancellationToken cancellation = default)
        {
            const string sql = @"
                UPDATE dbo.Variant
                   SET [Size]      = @Size,
                       UpdateDate  = SYSUTCDATETIME(),
                       Version     = Version + 1
                OUTPUT INSERTED.Id, INSERTED.ProductId, INSERTED.[Size],
                       INSERTED.CreationDate, INSERTED.UpdateDate, INSERTED.RemoveDate, INSERTED.Version, INSERTED.Guid, INSERTED.IsDeleted
                 WHERE ProductId = @ProductId AND Id = @Id AND IsDeleted = 0;";

            var updated = await _session.Connection.QuerySingleOrDefaultAsync<Variant>(
                new CommandDefinition(sql, new { ProductId = parent.Id, entity.Id, entity.Size }, _session.Transaction, cancellationToken: cancellation));

            if (updated != null)
            {
                updated.Images = (await _productImageRepository.SyncCollection(entity, entity.Images, cancellation)).ToList();
                updated.Attributes = (await _productAttributeRepository.SyncCollection(entity, entity.Attributes, cancellation)).ToList();
            }

            return updated ?? throw new KeyNotFoundException($"Variant {entity.Id} no encontrado para actualizar.");
        }
    }
}
