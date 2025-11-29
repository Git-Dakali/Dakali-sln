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
    public class FieldRepository : RepositoryReferenceEntity<FieldGroup, Field>
    {
        private readonly ISession _session;

        public FieldRepository(ISession session)
        {
            _session = session;
        }

        public async override Task<IEnumerable<Field>> Get(FieldGroup fieldGroup, CancellationToken cancellation = default)
        {
            var sql = @"SELECT * FROM Field WHERE FieldGroupId = @FieldGroupId AND IsDeleted = 0 ORDER BY SortOrder, Id;";

            return (await _session.Connection.QueryAsync<Field>(new CommandDefinition(sql, new { FieldGroupId = fieldGroup.Id }, _session.Transaction, cancellationToken: cancellation))).ToList();
        }

        public async override Task<Field> Get(FieldGroup parent, long id, CancellationToken cancellation = default)
        {
            const string sql = @"
                SELECT *
                  FROM dbo.Variant
                 WHERE FieldGroupId = @FieldGroupId AND Id = @Id AND IsDeleted = 0;";

            var field = await _session.Connection.QuerySingleOrDefaultAsync<Field>(
                new CommandDefinition(sql, new { FieldGroupId = parent.Id, Id = id }, _session.Transaction, cancellationToken: cancellation));

            return field;
        }

        public async override Task Delete(FieldGroup parent, Field entity, CancellationToken cancellation = default)
        {
            var sql = @"
                UPDATE dbo.Field
                SET IsDeleted  = 1,
                    RemoveDate = SYSUTCDATETIME(),
                    UpdateDate = SYSUTCDATETIME(),
                    Version    = Version + 1 
                WHERE FieldGroupId = @FieldGroupId AND Id = @Id AND IsDeleted = 0;";

            await _session.Connection.ExecuteAsync(new CommandDefinition(sql, new { FieldGroupId = parent.Id, entity.Id }, _session.Transaction, cancellationToken: cancellation));
        }

        public async Task Delete(FieldGroup fieldGroup, CancellationToken cancellation = default)
        {
            var sql = @"DELETE dbo.Field WHERE FieldGroupId = @FieldGroupId AND IsDeleted = 0;";

            await _session.Connection.ExecuteAsync(new CommandDefinition(sql, new { FieldGroupId = fieldGroup.Id}, _session.Transaction, cancellationToken: cancellation));
        }

        public async override Task Delete(FieldGroup parent, IEnumerable<Field> entities, CancellationToken cancellation = default)
        {
            if (entities is null)
                return;

            foreach (var entity in entities)
                await Delete(parent, entity, cancellation);
        }

        public async override Task<Field> Create(FieldGroup parent, Field entity, CancellationToken cancellation = default)
        {
            var sql = @"
                INSERT INTO dbo.Field (FieldGroupId, Name, SortOrder, SearchString) 
                OUTPUT INSERTED.*
                VALUES(@FieldGroupId, @Name, @SortOrder, @SearchString);";

            entity.SearchString = entity.ToString();
            return await _session.Connection.QuerySingleAsync<Field>(new CommandDefinition(sql, new { FieldGroupId = parent.Id, entity.Name, entity.SortOrder, entity.SearchString }, _session.Transaction, cancellationToken: cancellation));
        }

        public async override Task<Field> Update(FieldGroup parent, Field entity, CancellationToken cancellation = default)
        {
            const string sql = @"
                UPDATE dbo.Field
                   SET [Name]      = @Name,
                       SortOrder   = @SortOrder,
                       SearchString = @SearchString,
                       UpdateDate  = SYSUTCDATETIME(),
                       Version     = Version + 1
                OUTPUT INSERTED.*
                 WHERE FieldGroupId = @FieldGroupId AND Id = @Id AND IsDeleted = 0;";

            entity.SearchString = entity.ToString();
            var updated = await _session.Connection.QuerySingleOrDefaultAsync<Field>(
                new CommandDefinition(sql, new { FieldGroupId = parent.Id, entity.Id, entity.Name, entity.SortOrder, entity.SearchString }, _session.Transaction, cancellationToken: cancellation));

            return updated ?? throw new KeyNotFoundException($"Field {entity.Id} no encontrado para actualizar.");
        }

        public async override Task<bool> HasChanges(Field entity, Field persited)
        {
            return entity.Id != persited.Id ||
                entity.Name != persited.Name ||
                entity.SortOrder != persited.SortOrder;
        }
    }
}
