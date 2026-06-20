using DK.Domain.Products;
using DK.Repositories.Products;
using DK.Validator.Products;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Process.Product
{
    public class CategoryProcess
    {
        private CategoryRepository _categoryRepository;

        public CategoryProcess(CategoryRepository categoryRepository) 
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<Category>> GetAll(CancellationToken cancellationToken = default)
        {
            return await _categoryRepository.GetAll(cancellationToken);
        }

        public async Task<Category> Get(long id, CancellationToken cancellationToken = default)
        {
            return await _categoryRepository.Get(id, cancellationToken);
        }

        public async Task<Category> Get(string code, CancellationToken cancellationToken = default)
        {
            return await _categoryRepository.Get(code, cancellationToken);
        }

        public async Task<Category> Create(Category category, CancellationToken cancellationToken = default)
        {
            var validator = new CategoryValidator(_categoryRepository);
            await validator.Create(category, cancellationToken);

            return await _categoryRepository.Create(category, cancellationToken);
        }

        public async Task<Category> Update(Category category, CancellationToken cancellationToken = default)
        {
            var validator = new CategoryValidator(_categoryRepository);
            await validator.Update(category, cancellationToken);

            return await _categoryRepository.Update(category, cancellationToken);
        }

        public async Task Delete(Category category, CancellationToken cancellationToken = default)
        {
            var validator = new CategoryValidator(_categoryRepository);
            await validator.Delete(category, cancellationToken);
            await _categoryRepository.Delete(category, cancellationToken);
        }
    }
}
