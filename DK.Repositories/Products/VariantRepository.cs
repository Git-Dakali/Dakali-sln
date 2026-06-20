using Dakali.Interface.Connection;
using Dapper;
using DK.Domain.Products;
using DK.Repositories.Base;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Repositories.Products
{
    public class VariantRepository : RepositoryReferenceEntity<Product, Variant>
    {
        private readonly ISession _session;

        public VariantRepository(ISession session)
        {
            _session = session;
        }

        public async override Task<Variant> Create(Product parent, Variant entity, CancellationToken cancellation = default)
        {
            const string sql = @"
                INSERT INTO dbo.Variant (ProductId, [Name], SortOrder, SearchString)
                OUTPUT INSERTED.*
                VALUES (@ProductId, @Name, @SortOrder, @SearchString);";
            entity.SearchString = entity.ToString();
            var variant = await _session.Connection.QuerySingleAsync<Variant>(new CommandDefinition(sql, new { ProductId = parent.Id, entity.Name, entity.SortOrder, entity.SearchString }, _session.Transaction, cancellationToken: cancellation));
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
                 WHERE ProductId = @ProductId AND Id = @Id AND IsDeleted = 0;
            ";

            await _session.Connection.ExecuteAsync(
                new CommandDefinition(sql, new { ProductId = parent.Id, entity.Id }, _session.Transaction, cancellationToken: cancellation));
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

            return variants;
        }

        public async override Task<bool> HasChanges(Variant entity, Variant persited)
        {
            return entity.Id != persited.Id ||
                entity.Name != persited.Name ||
                entity.SortOrder != persited.SortOrder;
        }

        public async override Task<Variant> Update(Product parent, Variant entity, CancellationToken cancellation = default)
        {
            const string sql = @"
                UPDATE dbo.Variant
                   SET [Name]       = @Name,
                       SortOrder    = @SortOrder,
                       SearchString = @SearchString
                OUTPUT INSERTED.*
                 WHERE ProductId = @ProductId AND Id = @Id AND IsDeleted = 0;";

            entity.SearchString = entity.ToString();
            var updated = await _session.Connection.QuerySingleOrDefaultAsync<Variant>(
                new CommandDefinition(sql, new { ProductId = parent.Id, entity.Id, entity.Name, entity.SortOrder, entity.SearchString }, _session.Transaction, cancellationToken: cancellation));

            return updated ?? throw new KeyNotFoundException($"Variant {entity.Name} no encontrado para actualizar.");
        }
    }
}
