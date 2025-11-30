using Dakali.Interface.Connection;
using Dapper;
using DK.Domain.Products;
using DK.Repositories.Base;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Repositories.Products
{
    public class PropertyGroupRepository : RepositoryReferenceEntity<Variant, PropertyGroup>
    {
        private readonly ISession _session;
        private readonly PropertyRepository _propertyRepository;

        public PropertyGroupRepository(ISession session, PropertyRepository productAttributeRepository)
        {
            _session = session;
            _propertyRepository = productAttributeRepository;
        }

        public async override Task<PropertyGroup> Create(Variant parent, PropertyGroup entity, CancellationToken cancellation = default)
        {
            const string sql = @"
                INSERT INTO dbo.PropertyGroup (VariantId, [Name], [SortOrder], SearchString)
                OUTPUT INSERTED.*
                VALUES (@VariantId, @Name, @SortOrder, @SearchString);";
            
            entity.SearchString = entity.ToString();
            var group = await _session.Connection.QuerySingleAsync<PropertyGroup>(
                new CommandDefinition(sql, new { VariantId = parent.Id, entity.Name, entity.SortOrder, entity.SearchString }, _session.Transaction, cancellationToken: cancellation));

            group.Properties = await _propertyRepository.SyncCollection(group, entity.Properties, cancellation);
            return group;
        }

        public async override Task Delete(Variant parent, PropertyGroup entity, CancellationToken cancellation = default)
        {
            const string sql = @"
                UPDATE dbo.PropertyGroup
                   SET IsDeleted = 1,
                       RemoveDate = SYSUTCDATETIME(),
                       UpdateDate = SYSUTCDATETIME(),
                       Version = Version + 1
                 WHERE Id = @Id AND VariantId = @VariantId AND IsDeleted = 0;";

            entity.SearchString = entity.ToString();
            await _session.Connection.ExecuteAsync(
                new CommandDefinition(sql, new { entity.Id, VariantId = parent.Id }, _session.Transaction, cancellationToken: cancellation));
        }

        public async override Task Delete(Variant parent, IEnumerable<PropertyGroup> entities, CancellationToken cancellation = default)
        {
            if (entities is null)
                return;

            foreach (var entity in entities)
                await Delete(parent, entity, cancellation);
        }

        public async override Task<PropertyGroup> Get(Variant parent, long id, CancellationToken cancellation = default)
        {
            const string sql = @"
                SELECT Id, [Name], [SortOrder],
                    SearchString, CreationDate, UpdateDate, RemoveDate, Version, Guid, IsDeleted
                FROM dbo.PropertyGroup
                WHERE  Id = @Id AND VariantId = @VariantId AND IsDeleted = 0";

            var group = await _session.Connection.QuerySingleOrDefaultAsync<PropertyGroup>(
                new CommandDefinition(sql, new { Id = id, VariantId = parent.Id }, _session.Transaction, cancellationToken: cancellation));

            if (group != null)
                group.Properties = await _propertyRepository.Get(group, cancellation);

            return group;
        }

        public async override Task<IEnumerable<PropertyGroup>> Get(Variant parent, CancellationToken cancellation = default)
        {
            const string sql = @"
                SELECT Id, [Name], [SortOrder],
                    SearchString, CreationDate, UpdateDate, RemoveDate, Version, Guid, IsDeleted
                FROM dbo.PropertyGroup
                WHERE VariantId = @VariantId AND IsDeleted = 0
                ORDER BY Id;";

            var groups = await _session.Connection.QueryAsync<PropertyGroup>(
                new CommandDefinition(sql, new { VariantId = parent.Id }, _session.Transaction, cancellationToken: cancellation));

            foreach (var group in groups)
                group.Properties = await _propertyRepository.Get(group, cancellation);

            return groups;
        }

        public async override Task<bool> HasChanges(PropertyGroup entity, PropertyGroup persited)
        {
            return entity.Id != persited.Id
                || string.Compare(entity.Name, persited.Name, true) != 0
                || entity.SortOrder != persited.SortOrder
                || await _propertyRepository.HasChanges(persited, entity.Properties);
        }

        public async override Task<PropertyGroup> Update(Variant parent, PropertyGroup entity, CancellationToken cancellation = default)
        {
            const string sql = @"
                UPDATE dbo.PropertyGroup
                    SET [Name] = @Name,
                        [SortOrder] = @SortOrder,
                        SearchString = @SearchString,
                        UpdateDate = SYSUTCDATETIME(),
                        Version = Version + 1
                OUTPUT INSERTED.*
                    WHERE Id = @Id AND VariantId = @VariantId AND IsDeleted = 0;";

            entity.SearchString = entity.ToString();
            var updated = await _session.Connection.QuerySingleOrDefaultAsync<PropertyGroup>(
                new CommandDefinition(sql,
                    new { entity.Id, VariantId = parent.Id, entity.Name, entity.SortOrder, entity.SearchString },
                    _session.Transaction,
                    cancellationToken: cancellation));

            if(updated != null)
                updated.Properties = await _propertyRepository.SyncCollection(updated, entity.Properties, cancellation);

            return updated ?? throw new KeyNotFoundException($"El Grupo {entity.Id}-{entity.Name} no encontrado para la Variante {parent.Id}-{parent.Name}.");
            
        }
    }
}
