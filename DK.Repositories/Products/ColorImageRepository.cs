using Dakali.Interface.Connection;
using Dapper;
using DK.Domain.Products;
using DK.Repositories.Base;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Repositories.Products
{
    public class ColorImageRepository : RepositoryReferenceEntity<Color, Image>
    {

        private readonly ISession _session;
        private readonly StoredFileRepository _storedFileRepository;

        public ColorImageRepository(ISession session, StoredFileRepository storedFileRepository)
        {
            _session = session;
            _storedFileRepository = storedFileRepository;
        }

        public override async Task<Image> Get(Color parent, long id, CancellationToken cancellation = default)
        {
            const string sql = @"
                SELECT 
                    Id, 
                    ColorId, 
                    StoredFileId, 
                    IsPrimary, 
                    SortOrder,
                    SearchString,
                    CreationDate, 
                    UpdateDate, 
                    RemoveDate, 
                    Version, 
                    Guid, 
                    IsDeleted
                FROM dbo.Image
                WHERE Id = @Id AND ColorId = @ColorId AND IsDeleted = 0;";

            
            var row = await _session.Connection.QuerySingleOrDefaultAsync<dynamic>(
                new CommandDefinition(sql, new { Id = id, ColorId = parent.Id }, _session.Transaction, cancellationToken: cancellation));

            if (row is null)
                return null;

            var file = await _storedFileRepository.Get((long)row.StoredFileId, cancellation);

            return new Image
            {
                Id = (long)row.Id,
                Guid = row.Guid,
                SearchString = row.SearchString,
                CreationDate = row.CreationDate,
                UpdateDate = row.UpdateDate,
                RemoveDate = row.RemoveDate,
                Version = (long)row.Version,
                IsDeleted = row.IsDeleted,
                IsPrimary = (bool)row.IsPrimary,
                SortOrder = (int)row.SortOrder,
                File = file
            };
        }

        public override async Task<IEnumerable<Image>> Get(Color parent, CancellationToken cancellation = default)
        {
            const string sql = @"
                SELECT 
                    Id, 
                    ColorId, 
                    StoredFileId, 
                    IsPrimary, 
                    SortOrder,
                    SearchString,
                    CreationDate, 
                    UpdateDate, 
                    RemoveDate, 
                    Version, 
                    Guid, 
                    IsDeleted                    
                FROM dbo.Image 
                WHERE ColorId = @ColorId AND IsDeleted = 0
                ORDER BY SortOrder, Id;";

            var rows = await _session.Connection.QueryAsync<dynamic>(
                new CommandDefinition(sql, new { ColorId = parent.Id }, _session.Transaction, cancellationToken: cancellation));

            var list = new List<Image>();

            foreach (var row in rows)
            {
                var file = await _storedFileRepository.Get(row.StoredFileId, cancellation);
                list.Add(new Image
                {
                    Id = (long)row.Id,
                    Guid = row.Guid,
                    SearchString = row.SearchString,
                    CreationDate = row.CreationDate,
                    UpdateDate = row.UpdateDate,
                    RemoveDate = row.RemoveDate,
                    Version = (long)row.Version,
                    IsDeleted = row.IsDeleted,
                    IsPrimary = (bool)row.IsPrimary,
                    SortOrder = (int)row.SortOrder,
                    File = file
                });
            }
            return list;
        }

        public override async Task<Image> Create(Color parent, Image entity, CancellationToken cancellation = default)
        {
            if (entity.File.Id == 0)
                entity.File = await _storedFileRepository.Create(entity.File, cancellation);

            const string sql = @"
                INSERT INTO dbo.Image (ColorId, StoredFileId, IsPrimary, SortOrder, SearchString)
                OUTPUT INSERTED.Id, INSERTED.ColorId, INSERTED.StoredFileId, INSERTED.IsPrimary, INSERTED.SortOrder,
                       INSERTED.SearchString, INSERTED.CreationDate, INSERTED.UpdateDate, INSERTED.RemoveDate, INSERTED.Version, INSERTED.Guid, INSERTED.IsDeleted
                VALUES (@ColorId, @StoredFileId, @IsPrimary, @SortOrder, @SearchString);";

            entity.SearchString = entity.ToString();
            var row = await _session.Connection.QuerySingleAsync<dynamic>(
                new CommandDefinition(sql,
                    new
                    {
                        ColorId = parent.Id,
                        StoredFileId = entity.File.Id,
                        entity.IsPrimary,
                        entity.SortOrder,
                        entity.SearchString
                    },
                    _session.Transaction,
                    cancellationToken: cancellation));
            
            return new Image
            {
                Id = (long)row.Id,
                Guid = row.Guid,
                CreationDate = row.CreationDate,
                UpdateDate = row.UpdateDate,
                RemoveDate = row.RemoveDate,
                Version = (long)row.Version,
                IsDeleted = (bool)row.IsDeleted,
                IsPrimary = (bool)row.IsPrimary,
                SortOrder = (int)row.SortOrder,
                File = entity.File
            };
        }

        public override async Task<Image> Update(Color parent, Image entity, CancellationToken cancellation = default)
        {
            var imageOld = await Get(parent, entity.Id, cancellation);
            const string sqlUp = @"
                UPDATE dbo.Image
                   SET StoredFileId = @StoredFileId,
                       IsPrimary    = @IsPrimary,
                       SortOrder    = @SortOrder,
                       SearchString = @SearchString,
                       UpdateDate   = SYSUTCDATETIME(),
                       Version      = Version + 1
                OUTPUT INSERTED.Id, INSERTED.ColorId, INSERTED.StoredFileId, INSERTED.IsPrimary, INSERTED.SortOrder,
                       INSERTED.CreationDate, INSERTED.UpdateDate, INSERTED.RemoveDate, INSERTED.Version, INSERTED.Guid, INSERTED.IsDeleted
                WHERE Id = @Id AND ColorId = @ColorId AND IsDeleted = 0;";

            entity.SearchString = entity.ToString();
            var row = await _session.Connection.QuerySingleAsync<dynamic>(
                new CommandDefinition(sqlUp,
                    new
                    {
                        entity.Id, ColorId = parent.Id, StoredFileId = entity.File.Id, entity.IsPrimary, entity.SortOrder, entity.SearchString
                    },
                    _session.Transaction,
                    cancellationToken: cancellation));

            if (row is null)
                throw new KeyNotFoundException($"Image {entity.Id} no encontrado para actualizar.");

            var file = await _storedFileRepository.Get(row.StoredFileId, cancellation);
            var updateImage = new Image
            {
                Id = (long)row.Id,
                Guid = row.Guid,
                SearchString = entity.SearchString,
                CreationDate = row.CreationDate,
                UpdateDate = row.UpdateDate,
                RemoveDate = row.RemoveDate,
                Version = row.Version,
                IsDeleted = row.IsDeleted,
                IsPrimary = (bool)row.IsPrimary,
                SortOrder = (int)row.SortOrder,
                File = file
            };

            if (file.Id != imageOld.File.Id)
                await _storedFileRepository.Delete(imageOld.File, cancellation);

            return updateImage;
        }

        public override async Task Delete(Color parent, Image entity, CancellationToken cancellation = default)
        {
            const string sqlDelImg = @"
                UPDATE dbo.Image
                   SET IsDeleted = 1,
                       RemoveDate = SYSUTCDATETIME(),
                       UpdateDate = SYSUTCDATETIME(),
                       Version    = Version + 1
                 WHERE Id = @Id AND ColorId = @ColorId AND IsDeleted = 0;";
            await _session.Connection.ExecuteAsync(
                new CommandDefinition(sqlDelImg, new { entity.Id, ColorId = parent.Id }, _session.Transaction, cancellationToken: cancellation));

            await _storedFileRepository.Delete(entity.File, cancellation);
        }

        public override async Task Delete(Color parent, IEnumerable< Image> entities, CancellationToken cancellation = default)
        {
            if (entities is null)
                return;

            foreach (var entity in entities)
                await Delete(parent, entity, cancellation);
        }

        public async override Task<bool> HasChanges(Image entity, Image persisted)
        {
            return entity.Id != persisted.Id
                || entity.IsPrimary != persisted.IsPrimary
                || entity.SortOrder != persisted.SortOrder
                || entity.File.Id != persisted.File.Id;
        }
    }
}
