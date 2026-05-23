using Dakali.Interface.Connection;
using Dapper;
using DK.Domain.RoadMaps;
using DK.Repositories.Interface.Base;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Repositories.RoadMaps
{
    public class DriverRepository : IRepository<Driver>
    {
        private ISession _session;
        public DriverRepository(ISession session)
        {
            _session = session;
        }

        public async Task<Driver> Create(Driver entity, CancellationToken cancellation = default)
        {
            var query = @"
            INSERT INTO dbo.Driver (FirstName, LastName, Dni, SearchString)
            OUTPUT INSERTED.*
            VALUES (@FirstName, @LastName, @Dni, @SearchString);";

            entity.SearchString = entity.ToString();
            return await _session.Connection.QuerySingleAsync<Driver>(new CommandDefinition(query, entity, transaction: _session.Transaction, cancellationToken: cancellation));
        }

        public async Task Delete(Driver entity, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.Driver
                   SET IsDeleted = 1,
                       RemoveDate = SYSUTCDATETIME(),
                       UpdateDate = SYSUTCDATETIME(),
                       Version = Version + 1
                 WHERE Id = @id AND IsDeleted = 0;";

            await _session.Connection.ExecuteAsync(new CommandDefinition(query, entity, transaction: _session.Transaction, cancellationToken: cancellation));
        }

        public async Task<Driver> Get(long id, CancellationToken cancellation = default)
        {
            var query = "select * from dbo.Driver where IsDeleted = 0 AND Id = @Id";
            return await _session.Connection.QuerySingleOrDefaultAsync<Driver>(new CommandDefinition(query, new { Id = id }, transaction: _session.Transaction, cancellationToken: cancellation));
        }

        public async Task<IEnumerable<Driver>> GetAll(CancellationToken cancellation = default)
        {
            var query = "select * from dbo.Driver where IsDeleted = 0";
            return await _session.Connection.QueryAsync<Driver>(new CommandDefinition(query, new { }, transaction: _session.Transaction, cancellationToken: cancellation));
        }

        public async Task<Driver> Update(Driver entity, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.Driver
                SET 
                    FirstName = @FirstName,
                    LastName = @LastName,
                    Dni = @Dni,
                    SearchString = @SearchString,
                    UpdateDate = SYSUTCDATETIME(),
                    Version = Version + 1
                OUTPUT INSERTED.*
                WHERE Id = @Id AND IsDeleted = 0;
            ";

            entity.SearchString = entity.ToString();
            var newDriver = await _session.Connection.QuerySingleAsync<Driver>(new CommandDefinition(query, entity, transaction: _session.Transaction, cancellationToken: cancellation));


            return newDriver ?? throw new KeyNotFoundException($"El chofer {entity.FirstName} {entity.LastName} no se encontro para actualizar.");
        }
    }
}
