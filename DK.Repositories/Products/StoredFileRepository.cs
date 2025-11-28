using Dakali.Domine;
using Dakali.Interface.Connection;
using Dapper;
using DK.Repositories.Interface.Base;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Repositories.Products
{
    public class StoredFileRepository : IRepository<StoredFile>
    {
        private readonly ISession _session;

        public StoredFileRepository(ISession session)
        {
            _session = session;
        }

        public async Task<IEnumerable<StoredFile>> GetAll(CancellationToken cancellation = default)
        {
            const string sql = @"
                SELECT *
                FROM dbo.StoredFile
                WHERE IsDeleted = 0;
            ";

            return await _session.Connection.QueryAsync<StoredFile>(
                new CommandDefinition(sql, new {}, _session.Transaction, cancellationToken: cancellation));
        }

        public async Task<StoredFile?> Get(long id, CancellationToken cancellation = default)
        {
            const string sql = @"
                SELECT *
                FROM dbo.StoredFile
                WHERE Id = @Id AND IsDeleted = 0;
            ";

            return await _session.Connection.QuerySingleOrDefaultAsync<StoredFile>(
                new CommandDefinition(sql, new { Id = id }, _session.Transaction, cancellationToken: cancellation));
        }

        public async Task<StoredFile> Create(StoredFile entity, CancellationToken cancellation = default)
        {
            const string sql = @"
                INSERT INTO dbo.StoredFile (FileName, [ContentBase64], [Module], SearchString)
                OUTPUT INSERTED.*
                VALUES (@FileName, @ContentBase64, @Module, @SearchString);
            ";

            entity.SearchString = entity.ToString();
            return await _session.Connection.QuerySingleAsync<StoredFile>(
                new CommandDefinition(sql, entity, _session.Transaction, cancellationToken: cancellation));
        }

        public async Task<StoredFile> Update(StoredFile entity, CancellationToken cancellation = default)
        {
            const string sql = @"
                UPDATE dbo.StoredFile
                   SET FileName = @FileName,
                       [Module] = @Module,
                       SearchString = @SearchString,
                       UpdateDate = SYSUTCDATETIME(),
                       Version = Version + 1
                OUTPUT INSERTED.Id, INSERTED.FileName, INSERTED.[Module],
                       INSERTED.CreationDate, INSERTED.UpdateDate, INSERTED.RemoveDate,
                       INSERTED.Version, INSERTED.Guid, INSERTED.IsDeleted
                 WHERE Id = @Id AND IsDeleted = 0;
            ";

            entity.SearchString = entity.ToString();
            var file = await _session.Connection.QuerySingleAsync<StoredFile>(
                new CommandDefinition(sql, entity, _session.Transaction, cancellationToken: cancellation));

            return file ?? throw new KeyNotFoundException($"StoredFile {entity.Id}-{entity.FileName} no encontrado para actualizar.");
        }

        public async Task Delete(StoredFile entity, CancellationToken cancellation = default)
        {
            const string sql = @"
                UPDATE dbo.StoredFile
                   SET IsDeleted = 1,
                       RemoveDate = SYSUTCDATETIME(),
                       UpdateDate = SYSUTCDATETIME(),
                       Version = Version + 1
                 WHERE Id = @Id AND IsDeleted = 0;
            ";

            await _session.Connection.ExecuteAsync(
                new CommandDefinition(sql, entity, _session.Transaction, cancellationToken: cancellation));
        }

        public async Task<string> GetFileContentBase64(long id, CancellationToken cancellation = default)
        {
            const string sql = @"
                SELECT [ContentBase64]
                  FROM dbo.StoredFile
                 WHERE Id = @Id AND IsDeleted = 0;
            ";

            return await _session.Connection.ExecuteScalarAsync<string>(
                new CommandDefinition(sql, new { Id = id }, _session.Transaction, cancellationToken: cancellation));
        }
    }
}
