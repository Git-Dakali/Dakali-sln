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
    public class FieldRepository : RepositoryReferenceEntity<Product, Field>
    {
        private readonly ISession _session;

        public FieldRepository(ISession session)
        {
            _session = session;
        }

        public async override Task<IEnumerable<Field>> Get(Product parent, CancellationToken cancellation = default)
        {
            var sql = @"SELECT * FROM Field WHERE ProductId = @ProductId AND IsDeleted = 0 ORDER BY SortOrder, Id;";

            return (await _session.Connection.QueryAsync<Field>(new CommandDefinition(sql, new { ProductId = parent.Id }, _session.Transaction, cancellationToken: cancellation))).ToList();
        }

        public async override Task<Field> Get(Product parent, long id, CancellationToken cancellation = default)
        {
            const string sql = @"
                SELECT *
                  FROM dbo.Field
                 WHERE ProductId = @ProductId AND Id = @Id AND IsDeleted = 0;";

            var field = await _session.Connection.QuerySingleOrDefaultAsync<Field>(
                new CommandDefinition(sql, new { ProductId = parent.Id, Id = id }, _session.Transaction, cancellationToken: cancellation));

            return field;
        }

        public async override Task Delete(Product parent, Field entity, CancellationToken cancellation = default)
        {
            var sql = @"
                UPDATE dbo.Field
                SET IsDeleted  = 1,
                    RemoveDate = SYSUTCDATETIME(),
                    UpdateDate = SYSUTCDATETIME(),
                    Version    = Version + 1 
                WHERE ProductId = @ProductId AND Id = @Id AND IsDeleted = 0;";

            await _session.Connection.ExecuteAsync(new CommandDefinition(sql, new { ProductId = parent.Id, entity.Id }, _session.Transaction, cancellationToken: cancellation));
        }

        public async Task Delete(Product parent, CancellationToken cancellation = default)
        {
            var sql = @"DELETE dbo.Field WHERE ProductId = @ProductId AND IsDeleted = 0;";

            await _session.Connection.ExecuteAsync(new CommandDefinition(sql, new { ProductId = parent.Id }, _session.Transaction, cancellationToken: cancellation));
        }

        public async override Task Delete(Product parent, IEnumerable<Field> entities, CancellationToken cancellation = default)
        {
            if (entities is null)
                return;

            foreach (var entity in entities)
                await Delete(parent, entity, cancellation);
        }

        public async override Task<Field> Create(Product parent, Field entity, CancellationToken cancellation = default)
        {
            var sql = @"
                INSERT INTO dbo.Field (ProductId, Name, Value, SortOrder, SearchString) 
                OUTPUT INSERTED.*
                VALUES(@ProductId, @Name, @Value, @SortOrder, @SearchString);";

            entity.SearchString = entity.ToString();
            return await _session.Connection.QuerySingleAsync<Field>(new CommandDefinition(sql, new { ProductId = parent.Id, entity.Name, entity.Value, entity.SortOrder, entity.SearchString }, _session.Transaction, cancellationToken: cancellation));
        }

        public async override Task<Field> Update(Product parent, Field entity, CancellationToken cancellation = default)
        {
            const string sql = @"
                UPDATE dbo.Field
                   SET [Name]       = @Name,
                       Value        = @Value,
                       SortOrder    = @SortOrder,
                       SearchString = @SearchString,
                       UpdateDate   = SYSUTCDATETIME(),
                       Version      = Version + 1
                OUTPUT INSERTED.*
                 WHERE ProductId = @ProductId AND Id = @Id AND IsDeleted = 0;";

            entity.SearchString = entity.ToString();
            var updated = await _session.Connection.QuerySingleOrDefaultAsync<Field>(
                new CommandDefinition(sql, new { ProductId = parent.Id, entity.Id, entity.Name, entity.Value, entity.SortOrder, entity.SearchString }, _session.Transaction, cancellationToken: cancellation));

            return updated ?? throw new KeyNotFoundException($"Field {entity.Id} no encontrado para actualizar.");
        }

        public async override Task<bool> HasChanges(Field entity, Field persited)
        {
            return entity.Id != persited.Id ||
                entity.Name != persited.Name ||
                entity.Value != persited.Value ||
                entity.SortOrder != persited.SortOrder;
        }
    }
}
