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
        private readonly AttributeGroupRepository _attributeGroupRepository;
        private readonly ColorRepository _colorRepository;

        public VariantRepository(ISession session, AttributeGroupRepository attributeGroupRepository, ColorRepository colorRepository)
        {
            _session = session;
            _attributeGroupRepository = attributeGroupRepository;
            _colorRepository = colorRepository;
        }

        public async override Task<Variant> Create(Product parent, Variant entity, CancellationToken cancellation = default)
        {
            const string sql = @"
                INSERT INTO dbo.Variant (ProductId, [Name], Price, SalePrice, Active, SearchString)
                OUTPUT INSERTED.*
                VALUES (@ProductId, @Name, @Price, @SalePrice, @Active, @SearchString);";

            entity.SearchString = entity.ToString();
            var variant = await _session.Connection.QuerySingleAsync<Variant>(
                new CommandDefinition(sql, new { ProductId = parent.Id, entity.Name, entity.Price, entity.SalePrice, entity.Active, entity.SearchString }, _session.Transaction, cancellationToken: cancellation));
            variant.AttributeGroups = await _attributeGroupRepository.SyncCollection(variant, entity.AttributeGroups, cancellation);
            variant.ColorsHex = await _colorRepository.SyncCollection(variant, entity.ColorsHex, cancellation);
            return variant;
        }

        public async override Task Delete(Product parent, Variant entity, CancellationToken cancellation = default)
        {
            const string sql = @"
                UPDATE dbo.Variant
                   SET IsDeleted  = 1,
                       RemoveDate = SYSUTCDATETIME(),
                       UpdateDate = SYSUTCDATETIME(),
                       Version    = Version + 1
                 WHERE ProductId = @ProductId AND Id = @Id AND IsDeleted = 0;";

            await _session.Connection.ExecuteAsync(
                new CommandDefinition(sql, new { ProductId = parent.Id, entity.Id }, _session.Transaction, cancellationToken: cancellation));

            await _attributeGroupRepository.Delete(entity, entity.AttributeGroups);
            await _colorRepository.Delete(entity, entity.ColorsHex);

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
                SELECT *
                  FROM dbo.Variant
                 WHERE ProductId = @ProductId AND Id = @Id AND IsDeleted = 0;";

            var variant = await _session.Connection.QuerySingleOrDefaultAsync<Variant>(
                new CommandDefinition(sql, new { ProductId = parent.Id, Id = id }, _session.Transaction, cancellationToken: cancellation));

            if (variant != null)
            { 
                variant.AttributeGroups = (await _attributeGroupRepository.Get(variant)).ToList();
                variant.ColorsHex = await _colorRepository.Get(variant);
            }

            return variant;
        }

        public async override Task<IEnumerable<Variant>> Get(Product parent, CancellationToken cancellation = default)
        {
            const string sql = @"
                SELECT *
                  FROM dbo.Variant
                 WHERE ProductId = @ProductId AND IsDeleted = 0;";

            var variants = await _session.Connection.QueryAsync<Variant>(
                new CommandDefinition(sql, new { ProductId = parent.Id }, _session.Transaction, cancellationToken: cancellation));

            foreach (var variant in variants)
            {
                variant.AttributeGroups = await _attributeGroupRepository.Get(variant);
                variant.ColorsHex = await _colorRepository.Get(variant);
            }


            return variants;
        }

        public override bool HasChanges(Variant entity, Variant persited)
        {
            return entity.Id != persited.Id ||
                entity.ColorsHex != persited.ColorsHex ||
                entity.Price != persited.Price ||
                entity.SalePrice != persited.SalePrice;
        }

        public async override Task<Variant> Update(Product parent, Variant entity, CancellationToken cancellation = default)
        {
            const string sql = @"
                UPDATE dbo.Variant
                   SET [Name]       = @Name,
                       Price        = @Price,
                       SalePrice    = @SalePrice,
                       Active       = @Active,
                       SearchString = @SearchString,
                       UpdateDate   = SYSUTCDATETIME(),
                       Version      = Version + 1
                OUTPUT INSERTED.*
                 WHERE ProductId = @ProductId AND Id = @Id AND IsDeleted = 0;";

            entity.SearchString = entity.ToString();
            var updated = await _session.Connection.QuerySingleOrDefaultAsync<Variant>(
                new CommandDefinition(sql, new { ProductId = parent.Id, entity.Id, entity.Name, entity.Price, entity.SalePrice, entity.Active, entity.SearchString }, _session.Transaction, cancellationToken: cancellation));

            if (updated != null)
            { 
                updated.AttributeGroups = await _attributeGroupRepository.SyncCollection(entity, entity.AttributeGroups, cancellation);
                updated.ColorsHex = await _colorRepository.SyncCollection(entity, entity.ColorsHex, cancellation);
            }

            return updated ?? throw new KeyNotFoundException($"Variant {entity.Id} no encontrado para actualizar.");
        }
    }
}
