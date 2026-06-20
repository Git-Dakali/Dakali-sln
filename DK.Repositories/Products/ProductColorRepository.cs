using Dakali.Interface.Connection;
using Dapper;
using DK.Domain.Products;
using DK.Repositories.Base;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Repositories.Products
{
    public class ProductColorRepository : RepositoryReferenceEntity<Product, ProductColor>
    {
        private readonly ISession _session;
        private readonly ProductColorImageRepository _colorImageRepository;

        public ProductColorRepository(ISession session, ProductColorImageRepository productImageRepository)
        {
            _session = session;
            _colorImageRepository = productImageRepository;
        }

        public async override Task<ProductColor> Create(Product parent, ProductColor entity, CancellationToken cancellation = default)
        {
            var sql = @"
                INSERT INTO dbo.ProductColor (ProductId, Name, Hex, SortOrder, SearchString) 
                OUTPUT INSERTED.*
                VALUES(@ProductId, @Name, @Hex, @SortOrder, @SearchString);";

            entity.SearchString = entity.ToString();
            var color = await _session.Connection.QuerySingleAsync<ProductColor>(new CommandDefinition(sql, new { ProductId = parent.Id, entity.Name, entity.Hex, entity.SortOrder, entity.SearchString }, _session.Transaction, cancellationToken: cancellation));
            color.Images = await _colorImageRepository.SyncCollection(color, entity.Images, cancellation);

            return color;
        }

        public async override Task Delete(Product parent, ProductColor entity, CancellationToken cancellation = default)
        {
            var sql = @"
                UPDATE dbo.ProductColor
                SET IsDeleted  = 1,
                    RemoveDate = SYSUTCDATETIME(),
                    UpdateDate = SYSUTCDATETIME(),
                    Version    = Version + 1 
                WHERE ProductId = @ProductId AND Id = @Id AND IsDeleted = 0;";

            await _session.Connection.ExecuteAsync(new CommandDefinition(sql, new { ProductId = parent.Id, entity.Id }, _session.Transaction, cancellationToken: cancellation));
            await _colorImageRepository.Delete(entity, entity.Images, cancellation);
        }

        public async override Task Delete(Product parent, IEnumerable<ProductColor> entities, CancellationToken cancellation = default)
        {
            if (entities is null)
                return;

            foreach (var entity in entities)
                await Delete(parent, entity, cancellation);
        }

        public async override Task<ProductColor> Get(Product parent, long id, CancellationToken cancellation = default)
        {
            const string sql = @"
                SELECT *
                  FROM dbo.ProductColor
                 WHERE ProductId = @ProductId AND Id = @Id AND IsDeleted = 0;";

            var color = await _session.Connection.QuerySingleOrDefaultAsync<ProductColor>(
                new CommandDefinition(sql, new { ProductId = parent.Id, Id = id }, _session.Transaction, cancellationToken: cancellation));

            if (color != null)
                color.Images = await _colorImageRepository.Get(color);

            return color;
        }

        public async override Task<IEnumerable<ProductColor>> Get(Product parent, CancellationToken cancellation = default)
        {
            var sql = @"SELECT * FROM ProductColor WHERE ProductId = @ProductId AND IsDeleted = 0 ORDER BY SortOrder, Id;";
            var colors = await _session.Connection.QueryAsync<ProductColor>(new CommandDefinition(sql, new { ProductId = parent.Id }, _session.Transaction, cancellationToken: cancellation));

            foreach (var color in colors)
                color.Images = await _colorImageRepository.Get(color);
            
            return colors;
        }

        public async override Task<bool> HasChanges(ProductColor entity, ProductColor persited)
        {
            return entity.Id != persited.Id ||
                entity.Name != persited.Name ||
                entity.Hex != persited.Hex ||
                entity.SortOrder != persited.SortOrder ||
                await _colorImageRepository.HasChanges(persited, entity.Images);
        }

        public async override Task<ProductColor> Update(Product parent, ProductColor entity, CancellationToken cancellation = default)
        {
            const string sql = @"
                UPDATE dbo.ProductColor
                   SET Hex      = @Hex,
                       Name     = @Name,
                       SortOrder   = @SortOrder,
                       SearchString = @SearchString,
                       UpdateDate  = SYSUTCDATETIME(),
                       Version     = Version + 1
                OUTPUT INSERTED.*
                 WHERE ProductId = @ProductId AND Id = @Id AND IsDeleted = 0;";

            entity.SearchString = entity.ToString();
            var updated = await _session.Connection.QuerySingleOrDefaultAsync<ProductColor>(
                new CommandDefinition(sql, new { ProductId = parent.Id, entity.Id, entity.Name, entity.Hex, entity.SortOrder, entity.SearchString }, _session.Transaction, cancellationToken: cancellation));

            if (updated != null)
                updated.Images = await _colorImageRepository.SyncCollection(entity, entity.Images, cancellation);

            return updated ?? throw new KeyNotFoundException($"Color {entity.Name} no encontrado para actualizar.");
        }
    }
}
