using Dakali.Domine;
using Dakali.Interface.Connection;
using Dapper;
using DK.Domain.Products;
using DK.Repositories.Interface.Base;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Repositories.Products
{
    public class ProductRepository : IRepository<Product>
    {
        private ISession _session;
        private ModelRepository _modelRepository;
        private VariantRepository _variantRepository;

        public ProductRepository(ISession session, ModelRepository modelRepository, VariantRepository variantRepository)
        {
            _session = session;
            _modelRepository = modelRepository;
            _variantRepository = variantRepository;
        }

        public async Task<Product> Create(Product entity, CancellationToken cancellation = default)
        {
            var query = @"
            INSERT INTO dbo.Product (Name, Description, ModelId, SearchString)
            OUTPUT INSERTED.*
            VALUES (@Name, @Description, @ModelId, @SearchString);";

            entity.SearchString = entity.ToString();
            var product = await _session.Connection.QuerySingleAsync<Product>(query, new { entity.Name, entity.Description, ModelId = entity.Model.Id, entity.SearchString }, transaction: _session.Transaction);

            if (product != null)
                 await _variantRepository.SyncCollection(product, entity.Variants, cancellation);

            return await Get(product.Id, cancellation);
        }

        public async Task Delete(Product entity, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.Product
                   SET IsDeleted = 1,
                       RemoveDate = SYSUTCDATETIME(),
                       UpdateDate = SYSUTCDATETIME(),
                       Version = Version + 1
                 WHERE Id = @id AND IsDeleted = 0;";

            await _session.Connection.ExecuteAsync(query, entity, transaction: _session.Transaction);
            await _variantRepository.Delete(entity, entity.Variants, cancellation);
        }

        public async Task<IEnumerable<Product>> GetAll(CancellationToken cancellation = default)
        {
            var query = @"
                select * 
                from dbo.Product 
                where IsDeleted = 0";
            var rows = await _session.Connection.QueryAsync(query, new { }, transaction: _session.Transaction);

            if (rows == null)
                return Enumerable.Empty<Product>();

            var list = new List<Product>();

            foreach (var row in rows)
                list.Add(await Map(row));

            return list;
        }

        public async Task<ResultPage<Product>> GetPage(ProductFilter productFilter, CancellationToken cancellationToken = default)
        {
            if (productFilter is null || productFilter.CountRows <= 0 || productFilter.Page <= 0)
                return new ResultPage<Product>() { Count = 0, Values = new List<Product>() };

            var query = @" select * from dbo.Product where IsDeleted = 0";
            var queryCount = @" select COUNT(*) from dbo.Product where IsDeleted = 0";

            dynamic filter = new ExpandoObject();

            if (productFilter.Id != null)
            {
                query += " AND Id = @Id";
                queryCount += " AND Id = @Id";

                filter.Id = productFilter.Id;
            }

            if (!string.IsNullOrWhiteSpace(productFilter.SearchString))
            {
                query += " AND SearchString like @SearchString";
                queryCount += " AND SearchString like @SearchString";

                filter.SearchString = $"%{productFilter.SearchString}%";
            }

            query += @$"
                ORDER BY Id
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY;

                {queryCount}
            ";

            filter.Offset = (productFilter.Page - 1) * productFilter.CountRows;
            filter.PageSize = productFilter.CountRows;

            var results = await _session.Connection.QueryMultipleAsync(query, filter as object, transaction: _session.Transaction);
            var rowsDapper = results.Read().ToList();
            var count = results.Read<long>().Single();

            if (rowsDapper is null)
                return new ResultPage<Product>() { Count = 0, Values = new List<Product>() };

            var sales = new List<Product>();

            foreach (var row in rowsDapper)
                sales.Add(await Map(row));

            return new ResultPage<Product>() { Count = count, Values = sales };
        }

        public async Task<Product> Get(long id, CancellationToken cancellation = default)
        {
            var query = @"
                select *
                from dbo.Product 
                where IsDeleted = 0 AND Id = @Id";
            var rowProduct = await _session.Connection.QuerySingleOrDefaultAsync<dynamic>(query, new { Id = id }, transaction: _session.Transaction);

            if (rowProduct == null)
                return null;

            return await Map(rowProduct);
        }

        public async Task<Product> Update(Product entity, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.Product
                SET 
                    Name = @Name,
                    Description = @Description,
                    ModelId = @ModelId,
                    SearchString = @SearchString,
                    UpdateDate = SYSUTCDATETIME(),
                    Version = Version + 1
                OUTPUT INSERTED.*
                WHERE Id = @Id AND IsDeleted = 0;
            ";

            entity.SearchString = entity.ToString();
            await _session.Connection.QuerySingleAsync<Model>(query, new { entity.Id, entity.Name, entity.Description, ModelId = entity.Model.Id, entity.SearchString }, transaction: _session.Transaction);
            await _variantRepository.SyncCollection(entity, entity.Variants, cancellation);

            return await Get(entity.Id, cancellation) ?? throw new KeyNotFoundException($"Product {entity.Model.Code}-{entity.Name} no encontrado para actualizar.");
        }

        public async Task<Product> Map(dynamic rowDapper)
        {
            var product = new Product();

            product.Id = rowDapper.Id;
            product.SearchString = rowDapper.SearchString;
            product.CreationDate = rowDapper.CreationDate;
            product.UpdateDate = rowDapper.UpdateDate;
            product.RemoveDate = rowDapper.RemoveDate;
            product.IsDeleted = rowDapper.IsDeleted;
            product.Guid = rowDapper.Guid;
            product.Version = rowDapper.Version;
            product.Name = rowDapper.Name;
            product.Description = rowDapper.Description;
            product.Model = await _modelRepository.Get(rowDapper.ModelId);
            product.Variants = await _variantRepository.Get(product);

            return product;
        }
    }
}
