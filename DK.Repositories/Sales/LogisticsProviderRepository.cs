using Dakali.Interface.Connection;
using Dapper;
using DK.Domain.Sales;
using DK.Repositories.Interface.Base;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Repositories.Sales
{
    public class LogisticsProviderRepository : IRepositoryCode<LogisticsProvider>
    {
        private ISession _session;
        public LogisticsProviderRepository(ISession session)
        {
            _session = session;
        }

        public async Task<LogisticsProvider> Create(LogisticsProvider entity, CancellationToken cancellation = default)
        {
            var query = @"
            INSERT INTO dbo.LogisticsProvider (Code, Name, IsInHouse, SearchString)
            OUTPUT INSERTED.*
            VALUES (@Code, @Name, @IsInHouse, @SearchString);";

            entity.SearchString = entity.ToString();
            return await _session.Connection.QuerySingleAsync<LogisticsProvider>(new CommandDefinition(query, entity, transaction: _session.Transaction, cancellationToken: cancellation));
        }

        public async Task Delete(LogisticsProvider entity, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.LogisticsProvider
                   SET IsDeleted = 1,
                       RemoveDate = SYSUTCDATETIME(),
                       UpdateDate = SYSUTCDATETIME(),
                       Version = Version + 1
                 WHERE Id = @id AND IsDeleted = 0;";

            await _session.Connection.ExecuteAsync(new CommandDefinition(query, entity, transaction: _session.Transaction, cancellationToken: cancellation));
        }

        public async Task<LogisticsProvider> Get(string code, CancellationToken cancellation = default)
        {
            var query = "select * from dbo.LogisticsProvider where IsDeleted = 0 AND Code = @Code";
            return await _session.Connection.QuerySingleOrDefaultAsync<LogisticsProvider>(new CommandDefinition(query, new { Code = code }, transaction: _session.Transaction, cancellationToken: cancellation));
        }

        public async Task<LogisticsProvider> Get(long id, CancellationToken cancellation = default)
        {
            var query = "select * from dbo.LogisticsProvider where IsDeleted = 0 AND Id = @Id";
            return await _session.Connection.QueryFirstOrDefaultAsync<LogisticsProvider>(new CommandDefinition(query, new { Id = id }, transaction: _session.Transaction, cancellationToken: cancellation));
        }

        public async Task<IEnumerable<LogisticsProvider>> GetAll(CancellationToken cancellation = default)
        {
            var query = "select * from dbo.LogisticsProvider where IsDeleted = 0";
            return await _session.Connection.QueryAsync<LogisticsProvider>(new CommandDefinition(query, new { }, transaction: _session.Transaction, cancellationToken: cancellation));
        }

        public async Task<LogisticsProvider> Update(LogisticsProvider entity, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.LogisticsProvider
                SET 
                    Name = @Name,
                    IsInHouse = @IsInHouse,
                    SearchString = @SearchString,
                    UpdateDate = SYSUTCDATETIME(),
                    Version = Version + 1
                OUTPUT INSERTED.*
                WHERE Id = @Id AND IsDeleted = 0;
            ";

            entity.SearchString = entity.ToString();
            var newLogisticsProvider = await _session.Connection.QuerySingleAsync<LogisticsProvider>(new CommandDefinition(query, entity, transaction: _session.Transaction, cancellationToken: cancellation));


            return newLogisticsProvider ?? throw new KeyNotFoundException($"El Proveedor Logistico {entity.Name} no se encontro para actualizar.");
        }
    }
}
