using Dakali.Interface.Connection;
using Dapper;
using DK.Domain.Products;
using DK.Repositories.Interface.Base;
using System.Collections.Generic;
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
            INSERT INTO dbo.Product (Name, Description, ModelId)
            OUTPUT INSERTED.*
            VALUES (@Name, @Description, @ModelId);";

            var product = await _session.Connection.QuerySingleAsync<Product>(query, new { entity.Name, entity.Description, entity.Model.Id }, transaction: _session.Transaction);

            if (product != null)
                 await _variantRepository.SyncCollection(entity, entity.Variants, cancellation);

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

        public async Task<Product> Get(long id, CancellationToken cancellation = default)
        {
            var query = @"
                select Id, Name, Description, ModelId, CreationDate, RemoveDate, UpdateDate, Version, Guid, IsDeleted 
                from dbo.Product 
                where IsDeleted = 0 AND Id = @Id";
            var product = await _session.Connection.QuerySingleOrDefaultAsync<dynamic>(query, new { Id = id }, transaction: _session.Transaction);

            if (product == null)
                return null;

            return new Product()
            {
                Id = product.Id,
                CreationDate = product.CreationDate,
                UpdateDate = product.UpdateDate,
                RemoveDate = product.RemoveDate,
                IsDeleted = product.IsDeleted,
                Guid = product.Guid,
                Version = product.Version,
                Name = product.Code,
                Description = product.Description,
                Model = await _modelRepository.Get(product),
                Variants = await _variantRepository.Get(product)
            };
        }

        public async Task<Product> Update(Product entity, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.Product
                SET 
                    Name = @Name,
                    Description = @Description,
                    ModelId = @ModelId,
                    UpdateDate = SYSUTCDATETIME(),
                    Version = Version + 1
                OUTPUT INSERTED.*
                WHERE Id = @Id AND IsDeleted = 0;
            ";

            await _session.Connection.QuerySingleAsync<Model>(query, new { entity.Name, entity.Description, ModelId = entity.Model.Id }, transaction: _session.Transaction);
            await _variantRepository.SyncCollection(entity, entity.Variants, cancellation);

            return await Get(entity.Id, cancellation) ?? throw new KeyNotFoundException($"Product {entity.Model.Code}-{entity.Name} no encontrado para actualizar.");
        }
    }
}
