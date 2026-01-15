using Dakali.Interface.Connection;
using Dapper;
using DK.Domain.Locations;
using DK.Repositories.Interface.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Repositories.Locations
{
    public class ColumnRepository : IRepositoryCode<Column>
    {
        private ISession _session;
        public ColumnRepository(ISession session)
        {
            _session = session;
        }

        public async Task<Column> Create(Column entity, CancellationToken cancellation = default)
        {
            var query = @"
            INSERT INTO dbo.LocationColumn (Code, Name, SearchString)
            OUTPUT INSERTED.*
            VALUES (@Code, @Name, @SearchString);";

            entity.SearchString = entity.ToString();
            return await _session.Connection.QuerySingleAsync<Column>(query, entity, transaction: _session.Transaction);
        }

        public async Task Delete(Column entity, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.LocationColumn
                   SET IsDeleted = 1,
                       RemoveDate = SYSUTCDATETIME(),
                       UpdateDate = SYSUTCDATETIME(),
                       Version = Version + 1
                 WHERE Id = @id AND IsDeleted = 0;";

            await _session.Connection.ExecuteAsync(query, entity, transaction: _session.Transaction);
        }

        public async Task<Column> Get(string code, CancellationToken cancellation = default)
        {
            var query = "select * from dbo.LocationColumn where IsDeleted = 0 AND Code = @Code";
            return await _session.Connection.QuerySingleOrDefaultAsync<Column>(query, new { Code = code }, transaction: _session.Transaction);
        }

        public async Task<Column> Get(long id, CancellationToken cancellation = default)
        {
            var query = "select * from dbo.LocationColumn where IsDeleted = 0 AND Id = @Id";
            return await _session.Connection.QuerySingleOrDefaultAsync<Column>(query, new { Id = id }, transaction: _session.Transaction);
        }

        public async Task<IEnumerable<Column>> GetAll(CancellationToken cancellation = default)
        {
            var query = "select * from dbo.LocationColumn where IsDeleted = 0";
            return await _session.Connection.QueryAsync<Column>(query, new { }, transaction: _session.Transaction);
        }

        public async Task<Column> Update(Column entity, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.LocationColumn
                SET 
                    Name = @Name,
                    SearchString = @SearchString,
                    UpdateDate = SYSUTCDATETIME(),
                    Version = Version + 1
                OUTPUT INSERTED.*
                WHERE Id = @Id AND IsDeleted = 0;
            ";

            entity.SearchString = entity.ToString();
            var newState = await _session.Connection.QuerySingleAsync<Column>(query, entity, transaction: _session.Transaction);


            return newState ?? throw new KeyNotFoundException($"La columna {entity.Id}-{entity.Name} no encontrado para actualizar.");
        }
    }
}
