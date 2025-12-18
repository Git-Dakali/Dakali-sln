using Dakali.Interface.Connection;
using Dapper;
using DK.Domain.Products;
using DK.Repositories.Interface.Base;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Repositories.Products
{
    public class StockStateRepository : IRepositoryCode<StockState>
    {
        private ISession _session;
        public StockStateRepository(ISession session)
        {
            _session = session;
        }

        public async Task<IEnumerable<StockState>> GetAll(CancellationToken cancellation = default)
        {
            var query = "select * from dbo.StockState where IsDeleted = 0";
            return await _session.Connection.QueryAsync<StockState>(query, new { }, transaction: _session.Transaction);
        }

        public async Task<StockState> Get(long id, CancellationToken cancellation = default)
        {
            var query = "select * from dbo.StockState where IsDeleted = 0 AND Id = @Id";
            return await _session.Connection.QuerySingleOrDefaultAsync<StockState>(query, new { Id = id }, transaction: _session.Transaction);
        }

        public async Task<StockState> Get(string code, CancellationToken cancellation = default)
        {
            var query = "select * from dbo.StockState where IsDeleted = 0 AND Code = @Code";
            return await _session.Connection.QuerySingleOrDefaultAsync<StockState>(query, new { Code = code }, transaction: _session.Transaction);
        }

        public async Task<StockState> Create(StockState state, CancellationToken cancellation = default)
        {
            var query = @"
            INSERT INTO dbo.StockState (Code, Name, SearchString)
            OUTPUT INSERTED.*
            VALUES (@Code, @Name, @SearchString);";

            state.SearchString = state.ToString();
            return await _session.Connection.QuerySingleAsync<StockState>(query, state, transaction: _session.Transaction);
        }

        public async Task<StockState> Update(StockState state, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.StockState
                SET 
                    Name = @Name,
                    SearchString = @SearchString,
                    UpdateDate = SYSUTCDATETIME(),
                    Version = Version + 1
                OUTPUT INSERTED.*
                WHERE Id = @Id AND IsDeleted = 0;
            ";

            state.SearchString = state.ToString();
            var newState = await _session.Connection.QuerySingleAsync<StockState>(query, state, transaction: _session.Transaction);


            return newState ?? throw new KeyNotFoundException($"El esstado {state.Id}-{state.Name} no encontrado para actualizar.");
        }

        public async Task Delete(StockState state, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.StockState
                   SET IsDeleted = 1,
                       RemoveDate = SYSUTCDATETIME(),
                       UpdateDate = SYSUTCDATETIME(),
                       Version = Version + 1
                 WHERE Id = @id AND IsDeleted = 0;";

            await _session.Connection.ExecuteAsync(query, state, transaction: _session.Transaction);
        }
    }
}
