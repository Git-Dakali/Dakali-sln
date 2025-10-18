using Dakali.Interface.Connection;
using Dapper;
using DK.Domain.Products;
using DK.Repositories.Base;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Repositories
{
    public class SizeRepository : RepositoryReferenceEntity<Model, Size>
    {
        private readonly ISession _session;

        public SizeRepository(ISession session)
        {
            _session = session;
        }

        public async override Task<Size> Create(Model parent, Size entity, CancellationToken cancellation = default)
        {
            var sql = @"
                INSERT INTO dbo.Size (ModelId, Name, SortOrder) 
                OUTPUT INSERTED.*
                VALUES(@ModelId, @Name, @SortOrder);";

            return await _session.Connection.QuerySingleAsync(new CommandDefinition(sql, new { ModelId = parent.Id, entity.Name, entity.SortOrder }, _session.Transaction, cancellationToken: cancellation));
        }

        public async override Task Delete(Model parent, Size entity, CancellationToken cancellation = default)
        {
            var sql = @"
                UPDATE dbo.Size
                SET IsDeleted  = 1,
                    RemoveDate = SYSUTCDATETIME(),
                    UpdateDate = SYSUTCDATETIME(),
                    Version    = Version + 1 
                WHERE ModelId = @ModelId AND Id = @Id AND IsDeleted = 0;";

            await _session.Connection.ExecuteAsync(new CommandDefinition(sql, new { ModelId = parent.Id, entity.Id }, _session.Transaction, cancellationToken: cancellation));
        }

        public async override Task Delete(Model parent, IEnumerable<Size> entities, CancellationToken cancellation = default)
        {
            if (entities is null)
                return;

            foreach (var entity in entities)
                await Delete(parent, entity, cancellation);
        }

        public async override Task<Size> Get(Model parent, long id, CancellationToken cancellation = default)
        {
            const string sql = @"
                SELECT *
                  FROM dbo.Size
                 WHERE ModelId = @ModelId AND Id = @Id AND IsDeleted = 0;";

            var size = await _session.Connection.QuerySingleOrDefaultAsync<Size>(
                new CommandDefinition(sql, new { ModelId = parent.Id, Id = id }, _session.Transaction, cancellationToken: cancellation));

            return size;
        }

        public async override Task<IEnumerable<Size>> Get(Model parent, CancellationToken cancellation = default)
        {
            var sql = @"SELECT * FROM Size WHERE ModelId = @ModelId AND IsDeleted = 0; ORDER BY SortOrder, Id;";

            return await _session.Connection.QueryAsync<Size>(new CommandDefinition(sql, new { ModelId = parent.Id }, _session.Transaction, cancellationToken: cancellation));
        }

        public override bool HasChanges(Size entity, Size persited)
        {
            return entity.Id != persited.Id ||
                entity.Name != persited.Name ||
                entity.SortOrder != persited.SortOrder;
        }

        public async override Task<Size> Update(Model parent, Size entity, CancellationToken cancellation = default)
        {
            const string sql = @"
                UPDATE dbo.Size
                   SET Name      = @Name,
                       SortOrder   = @SortOrder
                       UpdateDate  = SYSUTCDATETIME(),
                       Version     = Version + 1
                OUTPUT INSERTED.*
                 WHERE ModelId = @ModelId AND Id = @Id AND IsDeleted = 0;";

            var updated = await _session.Connection.QuerySingleOrDefaultAsync<Size>(
                new CommandDefinition(sql, new { ModelId = parent.Id, entity.Id, entity.Name, entity.SortOrder }, _session.Transaction, cancellationToken: cancellation));

            return updated ?? throw new KeyNotFoundException($"Size {entity.Id} no encontrado para actualizar.");
        }
    }
}
