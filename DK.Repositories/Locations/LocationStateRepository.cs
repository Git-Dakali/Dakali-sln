using Dakali.Interface.Connection;
using Dapper;
using DK.Domain.Locations;
using DK.Repositories.Interface.Base;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Repositories.Locations
{
    public class LocationStateRepository : IRepositoryCode<LocationState>
    {
        private ISession _session;
        public LocationStateRepository(ISession session)
        {
            _session = session;
        }

        public async Task<IEnumerable<LocationState>> GetAll(CancellationToken cancellation = default)
        {
            var query = "select * from dbo.LocationState where IsDeleted = 0";
            return await _session.Connection.QueryAsync<LocationState>(query, new { }, transaction: _session.Transaction);
        }

        public async Task<LocationState> Get(long id, CancellationToken cancellation = default)
        {
            var query = "select * from dbo.LocationState where IsDeleted = 0 AND Id = @Id";
            return await _session.Connection.QuerySingleOrDefaultAsync<LocationState>(query, new { Id = id }, transaction: _session.Transaction);
        }

        public async Task<LocationState> Get(string code, CancellationToken cancellation = default)
        {
            var query = "select * from dbo.LocationState where IsDeleted = 0 AND Code = @Code";
            return await _session.Connection.QuerySingleOrDefaultAsync<LocationState>(query, new { Code = code }, transaction: _session.Transaction);
        }

        public async Task<LocationState> Create(LocationState state, CancellationToken cancellation = default)
        {
            var query = @"
            INSERT INTO dbo.LocationState (Code, Name, SearchString)
            OUTPUT INSERTED.*
            VALUES (@Code, @Name, @SearchString);";

            state.SearchString = state.ToString();
            return await _session.Connection.QuerySingleAsync<LocationState>(query, state, transaction: _session.Transaction);
        }

        public async Task<LocationState> Update(LocationState state, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.LocationState
                SET 
                    Name = @Name,
                    SearchString = @SearchString,
                    UpdateDate = SYSUTCDATETIME(),
                    Version = Version + 1
                OUTPUT INSERTED.*
                WHERE Id = @Id AND IsDeleted = 0;
            ";

            state.SearchString = state.ToString();
            var newState = await _session.Connection.QuerySingleAsync<LocationState>(query, state, transaction: _session.Transaction);


            return newState ?? throw new KeyNotFoundException($"El estado {state.Id}-{state.Name} no encontrado para actualizar.");
        }

        public async Task Delete(LocationState state, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.LocationState
                   SET IsDeleted = 1,
                       RemoveDate = SYSUTCDATETIME(),
                       UpdateDate = SYSUTCDATETIME(),
                       Version = Version + 1
                 WHERE Id = @id AND IsDeleted = 0;";

            await _session.Connection.ExecuteAsync(query, state, transaction: _session.Transaction);
        }
    }
}
