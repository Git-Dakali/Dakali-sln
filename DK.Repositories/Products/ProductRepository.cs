using Dakali.Interface.Connection;
using Dapper;
using DK.Domain.Products;
using DK.Repositories.Interface.Base;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
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
            {
                var product = new Product();

                product.Id = row.Id;
                product.SearchString = row.SearchString;
                product.CreationDate = row.CreationDate;
                product.UpdateDate = row.UpdateDate;
                product.RemoveDate = row.RemoveDate;
                product.IsDeleted = row.IsDeleted;
                product.Guid = row.Guid;
                product.Version = row.Version;
                product.Name = row.Name;
                product.Description = row.Description;
                product.Model = await _modelRepository.Get(row.ModelId, cancellation);
                product.Variants = await _variantRepository.Get(product, cancellation);

                list.Add(product);
            }

            return list;
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

            var product = new Product();

            product.Id = rowProduct.Id;
            product.SearchString = rowProduct.SearchString;
            product.CreationDate = rowProduct.CreationDate;
            product.UpdateDate = rowProduct.UpdateDate;
            product.RemoveDate = rowProduct.RemoveDate;
            product.IsDeleted = rowProduct.IsDeleted;
            product.Guid = rowProduct.Guid;
            product.Version = rowProduct.Version;
            product.Name = rowProduct.Name;
            product.Description = rowProduct.Description;
            product.Model = await _modelRepository.Get(rowProduct.ModelId);
            product.Variants = await _variantRepository.Get(product);

            return product;
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
    }
}
