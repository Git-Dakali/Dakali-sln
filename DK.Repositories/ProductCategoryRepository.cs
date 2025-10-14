using Dakali;
using Dakali.Interface.Connection;
using Dapper;
using DK.Model;
using DK.Repositories.Interface.Base;
using System.Security.Cryptography.Pkcs;
using System.Threading.Tasks;

namespace DK.Repositories
{
    public class ProductCategoryRepository: IRepository<ProductCategory>
    {
        private ISession _session;
        public ProductCategoryRepository(ISession session)
        {
            _session = session;
        }

        public async Task<ProductCategory> Get(long id)
        {
            var query = "select * from dbo.ProductCategory where IsDeleted = 0 AND Id = @Id";
            return await _session.Connection.QuerySingleOrDefaultAsync<ProductCategory>(query, new { Id = id }, transaction: _session.Transaction);
        }

        public async Task<ProductCategory> Create(ProductCategory productCategory)
        {
            var query = @"
            INSERT INTO dbo.ProductCategory (Code, Name)
            OUTPUT INSERTED.*
            VALUES (@Code, @Name);";

            return await _session.Connection.QuerySingleAsync<ProductCategory>(query, productCategory, transaction: _session.Transaction);
        }

        public async Task<ProductCategory> Update(ProductCategory productCategory)
        {
            var query = @"
                UPDATE dbo.ProductCategory
                SET 
                    Name = @Name,
                    UpdateDate = SYSUTCDATETIME(),
                    Version = Version + 1
                OUTPUT INSERTED.*
                WHERE Id = @Id AND IsDeleted = 0;
            ";

            return await _session.Connection.QuerySingleAsync<ProductCategory>(query, productCategory, transaction: _session.Transaction);
        }

        public async Task Delete(ProductCategory productCategory)
        {
            var query = @"
                UPDATE dbo.ProductCategory
                   SET IsDeleted = 1,
                       RemoveDate = SYSUTCDATETIME(),
                       UpdateDate = SYSUTCDATETIME(),
                       Version = Version + 1
                 WHERE Id = @id AND IsDeleted = 0;";

            await _session.Connection.ExecuteAsync(query, productCategory, transaction: _session.Transaction);
        }
    }
}
