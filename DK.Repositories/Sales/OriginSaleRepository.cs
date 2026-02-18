using Dakali.Interface.Connection;
using Dapper;
using DK.Domain.Sales;
using DK.Repositories.Interface.Base;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Repositories.Sales
{
    public class OriginSaleRepository : IRepository<OriginSale>
    {
        private ISession _session;
        public OriginSaleRepository(ISession session)
        {
            _session = session;
        }

        public async Task<OriginSale> Create(OriginSale entity, CancellationToken cancellation = default)
        {
            var query = @"
            INSERT INTO dbo.OriginSale (Code, Name, SearchString)
            OUTPUT INSERTED.*
            VALUES (@Code, @Name, @SearchString);";

            entity.SearchString = entity.ToString();
            return await _session.Connection.QuerySingleAsync<OriginSale>(query, entity, transaction: _session.Transaction);
        }

        public async Task Delete(OriginSale entity, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.OriginSale
                   SET IsDeleted = 1,
                       RemoveDate = SYSUTCDATETIME(),
                       UpdateDate = SYSUTCDATETIME(),
                       Version = Version + 1
                 WHERE Id = @id AND IsDeleted = 0;";

            await _session.Connection.ExecuteAsync(query, entity, transaction: _session.Transaction);
        }

        public async Task<OriginSale> Get(long id, CancellationToken cancellation = default)
        {
            var query = "select * from dbo.OriginSale where IsDeleted = 0 AND Id = @Id";
            return await _session.Connection.QuerySingleOrDefaultAsync<OriginSale>(query, new { Id = id }, transaction: _session.Transaction);
        }

        public async Task<OriginSale> Get(string code, CancellationToken cancellation = default)
        {
            var query = "select * from dbo.OriginSale where IsDeleted = 0 AND Code = @Code";
            return await _session.Connection.QuerySingleOrDefaultAsync<OriginSale>(query, new { Code = code }, transaction: _session.Transaction);
        }

        public async Task<IEnumerable<OriginSale>> GetAll(CancellationToken cancellation = default)
        {
            var query = "select * from dbo.OriginSale where IsDeleted = 0";
            return await _session.Connection.QueryAsync<OriginSale>(query, new { }, transaction: _session.Transaction);
        }

        public async Task<OriginSale> Update(OriginSale entity, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.OriginSale
                SET 
                    Name = @Name,
                    SearchString = @SearchString,
                    UpdateDate = SYSUTCDATETIME(),
                    Version = Version + 1
                OUTPUT INSERTED.*
                WHERE Id = @Id AND IsDeleted = 0;
            ";

            entity.SearchString = entity.ToString();
            var newOriginSale = await _session.Connection.QuerySingleAsync<OriginSale>(query, entity, transaction: _session.Transaction);


            return newOriginSale ?? throw new KeyNotFoundException($"Origen de Venta {entity.Name} no se encontro para actualizar.");
        }
    }
}
