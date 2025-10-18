using Dakali.Interface.Connection;
using Dapper;
using DK.Domain.Products;
using DK.Repositories.Base;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Repositories
{
    public class ColorRepository : RepositoryReferenceEntity<Variant, Color>
    {
        private readonly ISession _session;

        public ColorRepository(ISession session)
        {
            _session = session;
        }

        public async override Task<Color> Create(Variant parent, Color entity, CancellationToken cancellation = default)
        {
            var sql = @"
                INSERT INTO dbo.Color (VariantId, Hex, SortOrder) 
                OUTPUT INSERTED.*
                VALUES(@VariantId, @Hex, @SortOrder);";

            return await _session.Connection.QuerySingleAsync(new CommandDefinition(sql, new { VariantId = parent.Id, entity.Hex, entity.SortOrder }, _session.Transaction, cancellationToken: cancellation));
        }

        public async override Task Delete(Variant parent, Color entity, CancellationToken cancellation = default)
        {
            var sql = @"
                UPDATE dbo.Color
                SET IsDeleted  = 1,
                    RemoveDate = SYSUTCDATETIME(),
                    UpdateDate = SYSUTCDATETIME(),
                    Version    = Version + 1 
                WHERE VariantId = @VariantId AND Id = @Id AND IsDeleted = 0;";

            await _session.Connection.ExecuteAsync(new CommandDefinition(sql, new { VariantId = parent.Id, entity.Id }, _session.Transaction, cancellationToken: cancellation));
        }

        public async override Task Delete(Variant parent, IEnumerable<Color> entities, CancellationToken cancellation = default)
        {
            if (entities is null)
                return;

            foreach (var entity in entities)
                await Delete(parent, entity, cancellation);
        }

        public async override Task<Color> Get(Variant parent, long id, CancellationToken cancellation = default)
        {
            const string sql = @"
                SELECT *
                  FROM dbo.Color
                 WHERE VariantId = @VariantId AND Id = @Id AND IsDeleted = 0;";

            var color = await _session.Connection.QuerySingleOrDefaultAsync<Color>(
                new CommandDefinition(sql, new { VariantId = parent.Id, Id = id }, _session.Transaction, cancellationToken: cancellation));

            return color;
        }

        public async override Task<IEnumerable<Color>> Get(Variant parent, CancellationToken cancellation = default)
        {
            var sql = @"SELECT * FROM Color WHERE VariantId = @VariantId AND IsDeleted = 0; ORDER BY SortOrder, Id;";

            return await _session.Connection.QueryAsync<Color>(new CommandDefinition(sql, new { VariantId = parent.Id }, _session.Transaction, cancellationToken: cancellation));
        }

        public override bool HasChanges(Color entity, Color persited)
        {
            return entity.Id != persited.Id ||
                entity.Hex != persited.Hex ||
                entity.SortOrder != persited.SortOrder;
        }

        public async override Task<Color> Update(Variant parent, Color entity, CancellationToken cancellation = default)
        {
            const string sql = @"
                UPDATE dbo.Color
                   SET Hex      = @Hex,
                       SortOrder   = @SortOrder
                       UpdateDate  = SYSUTCDATETIME(),
                       Version     = Version + 1
                OUTPUT INSERTED.*
                 WHERE VariantId = @VariantId AND Id = @Id AND IsDeleted = 0;";

            var updated = await _session.Connection.QuerySingleOrDefaultAsync<Color>(
                new CommandDefinition(sql, new { VariantId = parent.Id, entity.Id, entity.Hex, entity.SortOrder }, _session.Transaction, cancellationToken: cancellation));

            return updated ?? throw new KeyNotFoundException($"Color {entity.Id} no encontrado para actualizar.");
        }
    }
}
