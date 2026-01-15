using Dakali.Interface.Connection;
using Dapper;
using DK.Domain.Locations;
using DK.Repositories.Interface.Base;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Repositories.Locations
{
    public class LevelRepository : IRepositoryCode<Level>
    {
        private ISession _session;
        public LevelRepository(ISession session)
        {
            _session = session;
        }

        public async Task<Level> Create(Level entity, CancellationToken cancellation = default)
        {
            var query = @"
            INSERT INTO dbo.Level (Code, Name, SearchString)
            OUTPUT INSERTED.*
            VALUES (@Code, @Name, @SearchString);";

            entity.SearchString = entity.ToString();
            return await _session.Connection.QuerySingleAsync<Level>(query, entity, transaction: _session.Transaction);
        }

        public async Task Delete(Level entity, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.Level
                   SET IsDeleted = 1,
                       RemoveDate = SYSUTCDATETIME(),
                       UpdateDate = SYSUTCDATETIME(),
                       Version = Version + 1
                 WHERE Id = @id AND IsDeleted = 0;";

            await _session.Connection.ExecuteAsync(query, entity, transaction: _session.Transaction);
        }

        public async Task<Level> Get(string code, CancellationToken cancellation = default)
        {
            var query = "select * from dbo.Level where IsDeleted = 0 AND Code = @Code";
            return await _session.Connection.QuerySingleOrDefaultAsync<Level>(query, new { Code = code }, transaction: _session.Transaction);
        }

        public async Task<Level> Get(long id, CancellationToken cancellation = default)
        {
            var query = "select * from dbo.Level where IsDeleted = 0 AND Id = @Id";
            return await _session.Connection.QuerySingleOrDefaultAsync<Level>(query, new { Id = id }, transaction: _session.Transaction);
        }

        public async Task<IEnumerable<Level>> GetAll(CancellationToken cancellation = default)
        {
            var query = "select * from dbo.Level where IsDeleted = 0";
            return await _session.Connection.QueryAsync<Level>(query, new { }, transaction: _session.Transaction);
        }

        public async Task<Level> Update(Level entity, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.Level
                SET 
                    Name = @Name,
                    SearchString = @SearchString,
                    UpdateDate = SYSUTCDATETIME(),
                    Version = Version + 1
                OUTPUT INSERTED.*
                WHERE Id = @Id AND IsDeleted = 0;
            ";

            entity.SearchString = entity.ToString();
            var newState = await _session.Connection.QuerySingleAsync<Level>(query, entity, transaction: _session.Transaction);


            return newState ?? throw new KeyNotFoundException($"El nivel {entity.Id}-{entity.Name} no encontrado para actualizar.");
        }
    }
}
