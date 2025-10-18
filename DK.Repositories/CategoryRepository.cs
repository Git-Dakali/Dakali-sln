using Dakali.Interface.Connection;
using Dapper;
using DK.Domain.Products;
using DK.Repositories.Interface.Base;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Repositories
{
    public class CategoryRepository : IRepository<Category>
    {
        private ISession _session;
        public CategoryRepository(ISession session)
        {
            _session = session;
        }

        public async Task<Category> Get(long id, CancellationToken cancellation = default)
        {
            var query = "select * from dbo.Category where IsDeleted = 0 AND Id = @Id";
            return await _session.Connection.QuerySingleOrDefaultAsync<Category>(query, new { Id = id }, transaction: _session.Transaction);
        }

        public async Task<Category> Create(Category Category, CancellationToken cancellation = default)
        {
            var query = @"
            INSERT INTO dbo.Category (Code, Name)
            OUTPUT INSERTED.*
            VALUES (@Code, @Name);";

            return await _session.Connection.QuerySingleAsync<Category>(query, Category, transaction: _session.Transaction);
        }

        public async Task<Category> Update(Category Category, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.Category
                SET 
                    Name = @Name,
                    UpdateDate = SYSUTCDATETIME(),
                    Version = Version + 1
                OUTPUT INSERTED.*
                WHERE Id = @Id AND IsDeleted = 0;
            ";

            var category = await _session.Connection.QuerySingleAsync<Category>(query, Category, transaction: _session.Transaction);

            return category?? throw new KeyNotFoundException($"Category {Category.Id}-{Category.Name} no encontrado para actualizar.");
        }

        public async Task Delete(Category Category, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.Category
                   SET IsDeleted = 1,
                       RemoveDate = SYSUTCDATETIME(),
                       UpdateDate = SYSUTCDATETIME(),
                       Version = Version + 1
                 WHERE Id = @id AND IsDeleted = 0;";

            await _session.Connection.ExecuteAsync(query, Category, transaction: _session.Transaction);
        }
    }
}
