using Dakali.Interface.Connection;
using Dapper;
using DK.Domain.Locations;
using DK.Repositories.Interface.Base;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Repositories.Locations
{
    public class HallwayRepository : IRepositoryCode<Hallway>
    {
        private ISession _session;
        public HallwayRepository(ISession session)
        {
            _session = session;
        }

        public async Task<Hallway> Create(Hallway entity, CancellationToken cancellation = default)
        {
            var query = @"
            INSERT INTO dbo.Hallway (Code, Name, SearchString)
            OUTPUT INSERTED.*
            VALUES (@Code, @Name, @SearchString);";

            entity.SearchString = entity.ToString();
            return await _session.Connection.QuerySingleAsync<Hallway>(query, entity, transaction: _session.Transaction);
        }

        public async Task Delete(Hallway entity, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.Hallway
                   SET IsDeleted = 1,
                       RemoveDate = SYSUTCDATETIME(),
                       UpdateDate = SYSUTCDATETIME(),
                       Version = Version + 1
                 WHERE Id = @id AND IsDeleted = 0;";

            await _session.Connection.ExecuteAsync(query, entity, transaction: _session.Transaction);
        }

        public async Task<Hallway> Get(string code, CancellationToken cancellation = default)
        {
            var query = "select * from dbo.Hallway where IsDeleted = 0 AND Code = @Code";
            return await _session.Connection.QuerySingleOrDefaultAsync<Hallway>(query, new { Code = code }, transaction: _session.Transaction);
        }

        public async Task<Hallway> Get(long id, CancellationToken cancellation = default)
        {
            var query = "select * from dbo.Hallway where IsDeleted = 0 AND Id = @Id";
            return await _session.Connection.QuerySingleOrDefaultAsync<Hallway>(query, new { Id = id }, transaction: _session.Transaction);
        }

        public async Task<IEnumerable<Hallway>> GetAll(CancellationToken cancellation = default)
        {
            var query = "select * from dbo.Hallway where IsDeleted = 0";
            return await _session.Connection.QueryAsync<Hallway>(query, new { }, transaction: _session.Transaction);
        }

        public async Task<Hallway> Update(Hallway entity, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.Hallway
                SET 
                    Name = @Name,
                    SearchString = @SearchString,
                    UpdateDate = SYSUTCDATETIME(),
                    Version = Version + 1
                OUTPUT INSERTED.*
                WHERE Id = @Id AND IsDeleted = 0;
            ";

            entity.SearchString = entity.ToString();
            var newState = await _session.Connection.QuerySingleAsync<Hallway>(query, entity, transaction: _session.Transaction);


            return newState ?? throw new KeyNotFoundException($"El pasillo {entity.Id}-{entity.Name} no encontrado para actualizar.");
        }
    }
}
