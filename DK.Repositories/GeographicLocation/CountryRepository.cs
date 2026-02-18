using Dakali.Interface.Connection;
using Dapper;
using DK.Domain.GeographicLocation;
using DK.Repositories.Interface.Base;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Repositories.GeographicLocation
{
    public class CountryRepository : IRepositoryCode<Country>
    {
        private ISession _session;
        public CountryRepository(ISession session)
        {
            _session = session;
        }

        public async Task<IEnumerable<Country>> GetAll(CancellationToken cancellation = default)
        {
            var query = "select * from dbo.Country where IsDeleted = 0";
            return await _session.Connection.QueryAsync<Country>(query, new { }, transaction: _session.Transaction);
        }

        public async Task<Country> Get(long id, CancellationToken cancellation = default)
        {
            var query = "select * from dbo.Country where IsDeleted = 0 AND Id = @Id";
            return await _session.Connection.QuerySingleOrDefaultAsync<Country>(query, new { Id = id }, transaction: _session.Transaction);
        }

        public async Task<Country> Get(string code, CancellationToken cancellation = default)
        {
            var query = "select * from dbo.Country where IsDeleted = 0 AND Code = @Code";
            return await _session.Connection.QuerySingleOrDefaultAsync<Country>(query, new { Code = code }, transaction: _session.Transaction);
        }

        public async Task<Country> Create(Country category, CancellationToken cancellation = default)
        {
            var query = @"
            INSERT INTO dbo.Country (Code, Name, SearchString)
            OUTPUT INSERTED.*
            VALUES (@Code, @Name, @SearchString);";

            category.SearchString = category.ToString();
            return await _session.Connection.QuerySingleAsync<Country>(query, category, transaction: _session.Transaction);
        }

        public async Task<Country> Update(Country category, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.Country
                SET 
                    Name = @Name,
                    SearchString = @SearchString,
                    UpdateDate = SYSUTCDATETIME(),
                    Version = Version + 1
                OUTPUT INSERTED.*
                WHERE Id = @Id AND IsDeleted = 0;
            ";

            category.SearchString = category.ToString();
            var newCategory = await _session.Connection.QuerySingleAsync<Country>(query, category, transaction: _session.Transaction);


            return newCategory ?? throw new KeyNotFoundException($"Pais {category.Id}-{category.Name} no encontrado para actualizar.");
        }

        public async Task Delete(Country Category, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.Country
                   SET IsDeleted = 1,
                       RemoveDate = SYSUTCDATETIME(),
                       UpdateDate = SYSUTCDATETIME(),
                       Version = Version + 1
                 WHERE Id = @id AND IsDeleted = 0;";

            await _session.Connection.ExecuteAsync(query, Category, transaction: _session.Transaction);
        }
    }
}
