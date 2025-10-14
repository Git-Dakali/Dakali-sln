using Dakali;
using Dapper;
using DK.Model;
using System.Threading.Tasks;

namespace DK.Repositories
{
    public class ProductCategoryRepository
    {
        public async Task<ProductCategory> Get(long id)
        {
            var query = "select * from dbo.ProductCategory where IsDeleted = 0 AND Id = @Id";
            return await ContextManager.Session.Connection.QuerySingleAsync<ProductCategory>(query, new { Id = id });
        }

        public async Task<ProductCategory> Create(ProductCategory productCategory)
        {
            var query = @"
            INSERT INTO dbo.ProductCategory (Code, Name)
            OUTPUT INSERTED.*
            VALUES (@Code, @Name);";

            return await ContextManager.Session.Connection.QuerySingleAsync<ProductCategory>(query, productCategory);
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

            return await ContextManager.Session.Connection.QuerySingleAsync<ProductCategory>(query, productCategory);
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

            await ContextManager.Session.Connection.ExecuteAsync(query, productCategory);
        }
    }
}
