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
    public class ColorRepository : RepositoryReferenceEntity<Variant, Color>
    {
        private readonly ISession _session;
        private readonly ColorImageRepository _colorImageRepository;

        public ColorRepository(ISession session, ColorImageRepository productImageRepository)
        {
            _session = session;
            _colorImageRepository = productImageRepository;
        }

        public async override Task<Color> Create(Variant parent, Color entity, CancellationToken cancellation = default)
        {
            var sql = @"
                INSERT INTO dbo.Color (VariantId, Name, Hex, SortOrder, SearchString) 
                OUTPUT INSERTED.*
                VALUES(@VariantId, @Name, @Hex, @SortOrder, @SearchString);";

            entity.SearchString = entity.ToString();
            var color = await _session.Connection.QuerySingleAsync<Color>(new CommandDefinition(sql, new { VariantId = parent.Id, entity.Name, entity.Hex, entity.SortOrder, entity.SearchString }, _session.Transaction, cancellationToken: cancellation));
            color.Images = await _colorImageRepository.SyncCollection(color, entity.Images, cancellation);

            return color;
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
            await _colorImageRepository.Delete(entity, entity.Images, cancellation);
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

            if (color != null)
                color.Images = await _colorImageRepository.Get(color);

            return color;
        }

        public async override Task<IEnumerable<Color>> Get(Variant parent, CancellationToken cancellation = default)
        {
            var sql = @"SELECT * FROM Color WHERE VariantId = @VariantId AND IsDeleted = 0 ORDER BY SortOrder, Id;";
            var colors = await _session.Connection.QueryAsync<Color>(new CommandDefinition(sql, new { VariantId = parent.Id }, _session.Transaction, cancellationToken: cancellation));

            foreach (var color in colors)
                color.Images = await _colorImageRepository.Get(color);
            
            return colors;
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
                       SortOrder   = @SortOrder,
                       SearchString = @SearchString,
                       UpdateDate  = SYSUTCDATETIME(),
                       Version     = Version + 1
                OUTPUT INSERTED.*
                 WHERE VariantId = @VariantId AND Id = @Id AND IsDeleted = 0;";

            entity.SearchString = entity.ToString();
            var updated = await _session.Connection.QuerySingleOrDefaultAsync<Color>(
                new CommandDefinition(sql, new { VariantId = parent.Id, entity.Id, entity.Hex, entity.SortOrder, entity.SearchString }, _session.Transaction, cancellationToken: cancellation));

            if (updated != null)
                updated.Images = await _colorImageRepository.SyncCollection(entity, entity.Images, cancellation);

            return updated ?? throw new KeyNotFoundException($"Color {entity.Id} no encontrado para actualizar.");
        }
    }
}
