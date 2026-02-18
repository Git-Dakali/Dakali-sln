using Dakali.Interface.Connection;
using Dapper;
using DK.Domain.Sales;
using DK.Repositories.Interface.Base;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Repositories.Sales
{
    public class TaxStatusRepository : IRepositoryCode<TaxStatus>
    {
        private ISession _session;
        public TaxStatusRepository(ISession session)
        {
            _session = session;
        }

        public async Task<TaxStatus> Create(TaxStatus entity, CancellationToken cancellation = default)
        {
            var query = @"
            INSERT INTO dbo.TaxStatus (Code, Name, SearchString)
            OUTPUT INSERTED.*
            VALUES (@Code, @Name, @SearchString);";

            entity.SearchString = entity.ToString();
            return await _session.Connection.QuerySingleAsync<TaxStatus>(query, entity, transaction: _session.Transaction);
        }

        public async Task Delete(TaxStatus entity, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.TaxStatus
                   SET IsDeleted = 1,
                       RemoveDate = SYSUTCDATETIME(),
                       UpdateDate = SYSUTCDATETIME(),
                       Version = Version + 1
                 WHERE Id = @id AND IsDeleted = 0;";

            await _session.Connection.ExecuteAsync(query, entity, transaction: _session.Transaction);
        }

        public async Task<TaxStatus> Get(string code, CancellationToken cancellation = default)
        {
            var query = "select * from dbo.TaxStatus where IsDeleted = 0 AND Code = @Code";
            return await _session.Connection.QuerySingleOrDefaultAsync<TaxStatus>(query, new { Code = code }, transaction: _session.Transaction);
        }

        public async Task<TaxStatus> Get(long id, CancellationToken cancellation = default)
        {
            var query = "select * from dbo.TaxStatus where IsDeleted = 0 AND Id = @Id";
            return await _session.Connection.QuerySingleOrDefaultAsync<TaxStatus>(query, new { Id = id }, transaction: _session.Transaction);
        }

        public async Task<IEnumerable<TaxStatus>> GetAll(CancellationToken cancellation = default)
        {
            var query = "select * from dbo.TaxStatus where IsDeleted = 0";
            return await _session.Connection.QueryAsync<TaxStatus>(query, new { }, transaction: _session.Transaction);
        }

        public async Task<TaxStatus> Update(TaxStatus entity, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.TaxStatus
                SET 
                    Name = @Name,
                    SearchString = @SearchString,
                    UpdateDate = SYSUTCDATETIME(),
                    Version = Version + 1
                OUTPUT INSERTED.*
                WHERE Id = @Id AND IsDeleted = 0;
            ";

            entity.SearchString = entity.ToString();
            var newState = await _session.Connection.QuerySingleAsync<TaxStatus>(query, entity, transaction: _session.Transaction);


            return newState ?? throw new Exception($"La Condicion Fiscal {entity.Code}-{entity.Name} no se encontro para actualizar.");
        }
    }
}
