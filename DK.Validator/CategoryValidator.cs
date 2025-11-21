using DK.Domain.Products;
using DK.Repositories.Products;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Validator
{
    public class CategoryValidator
    {
        private CategoryRepository _categoryRepository;

        public CategoryValidator(CategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException("CategoryRepository");
        }

        public async Task Create(Category category, CancellationToken cancellationToken = default)
        { 
            await Code(category, cancellationToken);
            await Name(category, cancellationToken);
        }

        public async Task Update(Category category, CancellationToken cancellationToken = default)
        {
            if (!(await Exist(category, cancellationToken)))
                throw new Exception($"No existe la categoria {category.Id}-{category.Code}");

            await Code(category, cancellationToken);
            await Name(category, cancellationToken);
        }

        public async Task Delete(Category category, CancellationToken cancellationToken = default)
        {
            if (!(await Exist(category, cancellationToken)))
                throw new Exception($"No existe la categoria {category.Id}-{category.Code}");
        }

        public async Task<bool> Exist(Category category, CancellationToken cancellationToken = default)
        {
            return (await _categoryRepository.Get(category.Id, cancellationToken)) != null;
        }

        public async Task Code(Category category, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(category.Code))
                throw new Exception("El codigo esta vacío.");

            if (category.Id > 0)
                return;

            var categoryPersisted = await _categoryRepository.Get(category.Code, cancellationToken);

            if (categoryPersisted != null)
                throw new Exception($"El codigo {category.Code} ya existe.");
        }

        public async Task Name(Category category, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(category.Name))
                throw new Exception("El nombre esta vacío.");
        }


    }
}
