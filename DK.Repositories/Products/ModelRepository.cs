using Dakali.Interface.Connection;
using Dapper;
using DK.Domain.Products;
using DK.Repositories.Interface.Base;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Repositories.Products
{
    public class ModelRepository : IRepository<Model>
    {
        private ISession _session;
        private CategoryRepository _categoryRepository;
        private FieldGroupRepository _fieldGroupRepository;

        public ModelRepository(ISession session, CategoryRepository categoryRepository, FieldGroupRepository fieldGroupRepository)
        {
            _session = session;
            _categoryRepository = categoryRepository;
            _fieldGroupRepository = fieldGroupRepository;
        }

        public async Task<Model> Create(Model entity, CancellationToken cancellation = default)
        {
            var query = @"
            INSERT INTO dbo.Model (Code, CategoryId)
            OUTPUT INSERTED.*
            VALUES (@Code, @CategoryId);";

            var model = await _session.Connection.QuerySingleAsync<Model>(query, new { entity.Code, CategoryId = entity.Category.Id }, transaction: _session.Transaction);

            if (model != null)
                model.FieldGroups = await _fieldGroupRepository.SyncCollection(entity, entity.FieldGroups, cancellation);

            return model;
        }

        public async Task Delete(Model entity, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.Model
                   SET IsDeleted = 1,
                       RemoveDate = SYSUTCDATETIME(),
                       UpdateDate = SYSUTCDATETIME(),
                       Version = Version + 1
                 WHERE Id = @id AND IsDeleted = 0;";

            await _session.Connection.ExecuteAsync(query, entity, transaction: _session.Transaction);
            await _fieldGroupRepository.Delete(entity, entity.FieldGroups, cancellation);
        }

        public async Task<Model> Get(long id, CancellationToken cancellation = default)
        {
            var query = @"
                select Id, Code, CategoryId, CreationDate, RemoveDate, UpdateDate, Version, Guid, IsDeleted 
                from dbo.Model 
                where IsDeleted = 0 AND Id = @Id";
            var model = await _session.Connection.QuerySingleOrDefaultAsync<dynamic>(query, new { Id = id }, transaction: _session.Transaction);

            if (model == null)
                return null;

            return new Model() { 
                Id = model.Id,
                Code = model.Code,
                CreationDate = model.CreationDate,
                UpdateDate = model.UpdateDate,
                RemoveDate = model.RemoveDate,
                IsDeleted = model.IsDeleted,
                Guid = model.Guid,
                Version = model.Version,
                Size = model.Size,
                FieldGroups = await _fieldGroupRepository.Get(model),
                Category = await _categoryRepository.Get(model.CategoryId)
            };
        }

        public async Task<Model> Update(Model entity, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.Model
                SET 
                    CategoryId = @CategoryId,
                    UpdateDate = SYSUTCDATETIME(),
                    Version = Version + 1
                OUTPUT INSERTED.*
                WHERE Id = @Id AND IsDeleted = 0;
            ";

            await _session.Connection.QuerySingleAsync<Model>(query, new { CategoryId = entity.Category.Id}, transaction: _session.Transaction);
            await _fieldGroupRepository.SyncCollection(entity, entity.FieldGroups, cancellation);
            
            return await Get(entity.Id, cancellation) ?? throw new KeyNotFoundException($"Model {entity.Code}-{entity.Category.Name} no encontrado para actualizar.");
        }
    }
}
