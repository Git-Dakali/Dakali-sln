using Dakali.Interface.Connection;
using Dapper;
using DK.Domain.Products;
using DK.Repositories.Interface.Base;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Repositories.Products
{
    public class ModelRepository : IRepositoryCode<Model>
    {
        private ISession _session;
        private CategoryRepository _categoryRepository;
        private FieldGroupRepository _fieldGroupRepository;
        private Model_VariantNameRepository _variantNameRepository;

        public ModelRepository(ISession session, CategoryRepository categoryRepository, FieldGroupRepository fieldGroupRepository, Model_VariantNameRepository sizeRepository)
        {
            _session = session;
            _categoryRepository = categoryRepository;
            _fieldGroupRepository = fieldGroupRepository;
            _variantNameRepository = sizeRepository;
        }

        public async Task<Model> Create(Model entity, CancellationToken cancellation = default)
        {
            var query = @"
            INSERT INTO dbo.Model (Code, CategoryId, SearchString)
            OUTPUT INSERTED.*
            VALUES (@Code, @CategoryId, @SearchString);";

            entity.SearchString = entity.ToString();
            var model = await _session.Connection.QuerySingleAsync<Model>(query, new { entity.Code, CategoryId = entity.Category.Id, entity.SearchString }, transaction: _session.Transaction);

            if (model != null)
            { 
                model.FieldGroups = await _fieldGroupRepository.SyncCollection(model, entity.FieldGroups, cancellation);
                model.VariantNames = await _variantNameRepository.SyncCollection(model, entity.VariantNames, cancellation);
            }

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

        public async Task<IEnumerable<Model>> GetAll(CancellationToken cancellation = default)
        {
            var query = @"
                select *
                from dbo.Model 
                where IsDeleted = 0";
            var rowsDapper = await _session.Connection.QueryAsync(query, new { }, transaction: _session.Transaction);

            if (rowsDapper == null)
                return Enumerable.Empty<Model>();

            var listModel = new List<Model>();
            foreach (var rowDapper in rowsDapper)
            {
                var newModel = new Model();
                newModel.Id = rowDapper.Id;
                newModel.Code = rowDapper.Code;
                newModel.SearchString = rowDapper.SearchString;
                newModel.CreationDate = rowDapper.CreationDate;
                newModel.UpdateDate = rowDapper.UpdateDate;
                newModel.RemoveDate = rowDapper.RemoveDate;
                newModel.IsDeleted = rowDapper.IsDeleted;
                newModel.Guid = rowDapper.Guid;
                newModel.Version = rowDapper.Version;
                newModel.VariantNames = await _variantNameRepository.Get(newModel, cancellation);
                newModel.FieldGroups = await _fieldGroupRepository.Get(newModel);
                newModel.Category = await _categoryRepository.Get(rowDapper.CategoryId);

                listModel.Add(newModel);
            }

            return listModel;
        }

        public async Task<Model> Get(long id, CancellationToken cancellation = default)
        {
            var query = @"
                select *
                from dbo.Model 
                where IsDeleted = 0 AND Id = @Id";
            var rowDapper = await _session.Connection.QuerySingleOrDefaultAsync<dynamic>(query, new { Id = id }, transaction: _session.Transaction);

            if (rowDapper == null)
                return null;

            var newModel = new Model();
            newModel.Id = rowDapper.Id;
            newModel.Code = rowDapper.Code;
            newModel.SearchString = rowDapper.SearchString;
            newModel.CreationDate = rowDapper.CreationDate;
            newModel.UpdateDate = rowDapper.UpdateDate;
            newModel.RemoveDate = rowDapper.RemoveDate;
            newModel.IsDeleted = rowDapper.IsDeleted;
            newModel.Guid = rowDapper.Guid;
            newModel.Version = rowDapper.Version;
            newModel.VariantNames = await _variantNameRepository.Get(newModel, cancellation);
            newModel.FieldGroups = await _fieldGroupRepository.Get(newModel, cancellation);
            newModel.Category = await _categoryRepository.Get(rowDapper.CategoryId, cancellation);

            return newModel;
        }

        public async Task<Model> Get(string code, CancellationToken cancellation = default)
        {
            var query = @"
                select Id, Code, CategoryId, CreationDate, RemoveDate, UpdateDate, Version, Guid, IsDeleted 
                from dbo.Model 
                where IsDeleted = 0 AND Code = @Code";
            var rowDapper = await _session.Connection.QuerySingleOrDefaultAsync<dynamic>(query, new { Code = code }, transaction: _session.Transaction);

            if (rowDapper == null)
                return null;

            var newModel = new Model();
            newModel.Id = rowDapper.Id;
            newModel.Code = rowDapper.Code;
            newModel.CreationDate = rowDapper.CreationDate;
            newModel.UpdateDate = rowDapper.UpdateDate;
            newModel.RemoveDate = rowDapper.RemoveDate;
            newModel.IsDeleted = rowDapper.IsDeleted;
            newModel.Guid = rowDapper.Guid;
            newModel.Version = rowDapper.Version;
            newModel.VariantNames = await _variantNameRepository.Get(newModel, cancellation);
            newModel.FieldGroups = await _fieldGroupRepository.Get(newModel, cancellation);
            newModel.Category = await _categoryRepository.Get(rowDapper.CategoryId, cancellation);

            return newModel;
        }

        public async Task<Model> Update(Model entity, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.Model
                SET 
                    CategoryId = @CategoryId,
                    SearchString = @SearchString,
                    UpdateDate = SYSUTCDATETIME(),
                    Version = Version + 1
                OUTPUT INSERTED.*
                WHERE Id = @Id AND IsDeleted = 0;
            ";

            entity.SearchString = entity.ToString();
            await _session.Connection.QuerySingleAsync<Model>(query, new { entity.Id, CategoryId = entity.Category.Id, entity.SearchString}, transaction: _session.Transaction);
            await _fieldGroupRepository.SyncCollection(entity, entity.FieldGroups, cancellation);
            await _variantNameRepository.SyncCollection(entity, entity.VariantNames, cancellation);

            return await Get(entity.Id, cancellation) ?? throw new KeyNotFoundException($"Model {entity.Code}-{entity.Category.Name} no se encontro para actualizar.");
        }
    }
}
