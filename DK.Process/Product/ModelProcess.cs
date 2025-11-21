using DK.Domain.Products;
using DK.Repositories.Products;
using DK.Validator;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Process.Product
{
    public class ModelProcess
    {
        private ModelRepository _modelRepository;
        private CategoryValidator _categoryValidator;

        public ModelProcess(ModelRepository modelRepository, CategoryValidator categoryValidator)
        {
            _modelRepository = modelRepository;
            _categoryValidator = categoryValidator;
        }

        public async Task<IEnumerable<Model>> GetAll(CancellationToken cancellationToken = default)
        {
            return await _modelRepository.GetAll(cancellationToken);
        }

        public async Task<Model> Get(long id, CancellationToken cancellationToken = default)
        {
            return await _modelRepository.Get(id, cancellationToken);
        }

        public async Task<Model> Get(string code, CancellationToken cancellationToken = default)
        {
            return await _modelRepository.Get(code, cancellationToken);
        }

        public async Task<Model> Create(Model model, CancellationToken cancellationToken = default)
        {
            var validator = new ModelValidator(_modelRepository, _categoryValidator);
            await validator.Create(model, cancellationToken);

            return await _modelRepository.Create(model, cancellationToken);
        }

        public async Task<Model> Update(Model model, CancellationToken cancellationToken = default)
        {
            var validator = new ModelValidator(_modelRepository, _categoryValidator);
            await validator.Update(model, cancellationToken);

            return await _modelRepository.Update(model, cancellationToken);
        }

        public async Task Delete(Model model, CancellationToken cancellationToken = default)
        {
            var validator = new ModelValidator(_modelRepository, _categoryValidator);
            await validator.Delete(model, cancellationToken);
            await _modelRepository.Delete(model, cancellationToken);
        }
    }
}
