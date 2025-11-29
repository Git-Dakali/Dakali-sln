using Dakali.Interface.Connection;
using Dapper;
using DK.Domain.Products;
using DK.Repositories.Base;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Repositories.Products
{
    public class AttributeGroupRepository : RepositoryReferenceEntity<Variant, AttributeGroup>
    {
        private readonly ISession _session;
        private readonly AttributeRepository _productAttributeRepository;

        public AttributeGroupRepository(ISession session, AttributeRepository productAttributeRepository)
        {
            _session = session;
            _productAttributeRepository = productAttributeRepository;
        }

        public async override Task<AttributeGroup> Create(Variant parent, AttributeGroup entity, CancellationToken cancellation = default)
        {
            const string sql = @"
                INSERT INTO dbo.AttributeGroup (VariantId, [Name], [SortOrder], SearchString)
                OUTPUT INSERTED.*
                VALUES (@VariantId, @Name, @SortOrder, @SearchString);";
            
            entity.SearchString = entity.ToString();
            var group = await _session.Connection.QuerySingleAsync<AttributeGroup>(
                new CommandDefinition(sql, new { VariantId = parent.Id, entity.Name, entity.SortOrder, entity.SearchString }, _session.Transaction, cancellationToken: cancellation));

            group.Attributes = await _productAttributeRepository.SyncCollection(group, entity.Attributes, cancellation);
            return group;
        }

        public async override Task Delete(Variant parent, AttributeGroup entity, CancellationToken cancellation = default)
        {
            const string sql = @"
                UPDATE dbo.AttributeGroup
                   SET IsDeleted = 1,
                       RemoveDate = SYSUTCDATETIME(),
                       UpdateDate = SYSUTCDATETIME(),
                       Version = Version + 1
                 WHERE Id = @Id AND VariantId = @VariantId AND IsDeleted = 0;";

            entity.SearchString = entity.ToString();
            await _session.Connection.ExecuteAsync(
                new CommandDefinition(sql, new { entity.Id, VariantId = parent.Id }, _session.Transaction, cancellationToken: cancellation));
        }

        public async override Task Delete(Variant parent, IEnumerable<AttributeGroup> entities, CancellationToken cancellation = default)
        {
            if (entities is null)
                return;

            foreach (var entity in entities)
                await Delete(parent, entity, cancellation);
        }

        public async override Task<AttributeGroup> Get(Variant parent, long id, CancellationToken cancellation = default)
        {
            const string sql = @"
                SELECT Id, [Name], [SortOrder],
                    SearchString, CreationDate, UpdateDate, RemoveDate, Version, Guid, IsDeleted
                FROM dbo.AttributeGroup
                WHERE  Id = @Id AND VariantId = @VariantId AND IsDeleted = 0";

            var group = await _session.Connection.QuerySingleOrDefaultAsync<AttributeGroup>(
                new CommandDefinition(sql, new { Id = id, VariantId = parent.Id }, _session.Transaction, cancellationToken: cancellation));

            if (group != null)
                group.Attributes = await _productAttributeRepository.Get(group);

            return group;
        }

        public async override Task<IEnumerable<AttributeGroup>> Get(Variant parent, CancellationToken cancellation = default)
        {
            const string sql = @"
                SELECT Id, [Name], [SortOrder],
                    SearchString, CreationDate, UpdateDate, RemoveDate, Version, Guid, IsDeleted
                FROM dbo.AttributeGroup
                WHERE VariantId = @VariantId AND IsDeleted = 0
                ORDER BY Id;";

            return await _session.Connection.QueryAsync<AttributeGroup>(
                new CommandDefinition(sql, new { VariantId = parent.Id }, _session.Transaction, cancellationToken: cancellation));
        }

        public async override Task<bool> HasChanges(AttributeGroup entity, AttributeGroup persited)
        {
            return entity.Id != persited.Id
                || string.Compare(entity.Name, persited.Name, true) != 0
                || entity.SortOrder != persited.SortOrder
                || await _productAttributeRepository.HasChanges(persited, entity.Attributes);
        }

        public async override Task<AttributeGroup> Update(Variant parent, AttributeGroup entity, CancellationToken cancellation = default)
        {
            const string sql = @"
                UPDATE dbo.AttributeGroup
                    SET [Name] = @Name,
                        [SortOrder] = @SortOrder,
                        SearchString = @SearchString,
                        UpdateDate = SYSUTCDATETIME(),
                        Version = Version + 1
                OUTPUT INSERTED.*
                    WHERE Id = @Id AND VariantId = @VariantId AND IsDeleted = 0;";

            entity.SearchString = entity.ToString();
            var updated = await _session.Connection.QuerySingleOrDefaultAsync<AttributeGroup>(
                new CommandDefinition(sql,
                    new { entity.Id, VariantId = parent.Id, entity.Name, entity.SortOrder, entity.SearchString },
                    _session.Transaction,
                    cancellationToken: cancellation));

            if(updated != null)
                updated.Attributes = await _productAttributeRepository.SyncCollection(updated, entity.Attributes, cancellation);

            return updated ?? throw new KeyNotFoundException($"El Grupo {entity.Id}-{entity.Name} no encontrado para la Variante {parent.Id}-{parent.Name}.");
            
        }
    }
}
