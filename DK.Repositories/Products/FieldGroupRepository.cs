using Dakali.Interface.Connection;
using Dapper;
using DK.Domain.Products;
using DK.Repositories.Base;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Repositories.Products
{
    public class FieldGroupRepository : RepositoryReferenceEntity<Model, FieldGroup>
    {
        private readonly ISession _session;
        private readonly FieldRepository _fieldRepfository;

        public FieldGroupRepository(ISession session, FieldRepository fieldRepository)
        {
            _session = session;
            _fieldRepfository = fieldRepository;
        }

        public override async Task<IEnumerable<FieldGroup>> Get(Model parent, CancellationToken cancellation = default)
        {
            const string sql = @"
                SELECT Id, [Name], ProductModelId, SortOrder,
                       CreationDate, UpdateDate, RemoveDate, Version, Guid, IsDeleted
                  FROM dbo.FieldGroup
                 WHERE ProductModelId = @ModelId AND IsDeleted = 0
                 ORDER BY SortOrder, Id;";

            var groups = await _session.Connection.QueryAsync<FieldGroup>(
                new CommandDefinition(sql, new { ModelId = parent.Id }, _session.Transaction, cancellationToken: cancellation));

            foreach (var group in groups)
                group.Fields = await _fieldRepfository.Get(group, cancellation);

            return groups;
        }

        public override async Task<FieldGroup> Get(Model parent, long id, CancellationToken cancellation = default)
        {
            const string sql = @"
                SELECT Id, [Name], ProductModelId, SortOrder,
                       CreationDate, UpdateDate, RemoveDate, Version, Guid, IsDeleted
                  FROM dbo.FieldGroup
                 WHERE Id = @Id AND ProductModelId = @ModelId AND IsDeleted = 0;";

            var entity = await _session.Connection.QuerySingleOrDefaultAsync<FieldGroup>(
                new CommandDefinition(sql, new { Id = id, ModelId = parent.Id }, _session.Transaction, cancellationToken: cancellation));

            if (entity != null)
                entity.Fields = await _fieldRepfository.Get(entity, cancellation);

            return entity;
        }

        public override async Task<FieldGroup> Create(Model parent, FieldGroup entity, CancellationToken cancellation = default)
        {
            const string sql = @"
                INSERT INTO dbo.FieldGroup (ProductModelId, [Name], SortOrder)
                OUTPUT INSERTED.Id, INSERTED.[Name], INSERTED.ProductModelId, INSERTED.SortOrder,
                       INSERTED.CreationDate, INSERTED.UpdateDate, INSERTED.RemoveDate, INSERTED.Version, INSERTED.Guid, INSERTED.IsDeleted
                VALUES (@ModelId, @Name, @SortOrder);";

            var persisted = await _session.Connection.QuerySingleAsync<FieldGroup>(
                new CommandDefinition(sql, new { ModelId = parent.Id, entity.Name, entity.SortOrder }, _session.Transaction, cancellationToken: cancellation));

            persisted.Fields = await _fieldRepfository.SyncCollection(persisted, entity.Fields, cancellation);

            return persisted;
        }

        public override async Task<FieldGroup> Update(Model parent, FieldGroup entity, CancellationToken cancellation = default)
        {
            const string sql = @"
                UPDATE dbo.FieldGroup
                   SET [Name]     = @Name,
                       SortOrder  = @SortOrder,
                       UpdateDate = SYSUTCDATETIME(),
                       Version    = Version + 1
                OUTPUT INSERTED.Id, INSERTED.[Name], INSERTED.ProductModelId, INSERTED.SortOrder,
                       INSERTED.CreationDate, INSERTED.UpdateDate, INSERTED.RemoveDate, INSERTED.Version, INSERTED.Guid, INSERTED.IsDeleted
                 WHERE Id = @Id AND ProductModelId = @ModelId AND IsDeleted = 0;";

            var updated = await _session.Connection.QuerySingleOrDefaultAsync<FieldGroup>(
                new CommandDefinition(sql, new { entity.Id, ModelId = parent.Id, entity.Name, entity.SortOrder }, _session.Transaction, cancellationToken: cancellation));

            if (updated != null)
                updated.Fields = await _fieldRepfository.SyncCollection(updated, entity.Fields);

            return updated ?? throw new KeyNotFoundException($"FieldGroup {entity.Id}-{entity.Name} no encontrado para ProductModel {parent.Id}.");
        }


        public override async Task Delete(Model parent, FieldGroup entity, CancellationToken cancellation = default)
        {
            const string sql = @"
                UPDATE dbo.FieldGroup
                   SET IsDeleted  = 1,
                       RemoveDate = SYSUTCDATETIME(),
                       UpdateDate = SYSUTCDATETIME(),
                       Version    = Version + 1
                 WHERE Id = @Id AND ProductModelId = @ModelId AND IsDeleted = 0;";

            await _session.Connection.ExecuteAsync(
                new CommandDefinition(sql, new { entity.Id, ModelId = parent.Id }, _session.Transaction, cancellationToken: cancellation));

            await _fieldRepfository.Delete(entity, cancellation);
        }

        public override bool HasChanges(FieldGroup entity, FieldGroup persisted)
        {
            return entity.Name != persisted.Name ||
                   entity.SortOrder != persisted.SortOrder;
        }

        public async override Task Delete(Model parent, IEnumerable<FieldGroup> entities, CancellationToken cancellation = default)
        {
            foreach (var entity in entities)
                await Delete(parent, entity, cancellation);
        }
    }
}
