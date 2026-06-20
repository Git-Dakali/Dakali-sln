using DK.Domain.Products;
using DK.Repositories.Products;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Validator.Products
{
    public class FieldValidator
    {
        public FieldRepository _fieldRepository;

        public FieldValidator(FieldRepository fieldRepository)
        {
            _fieldRepository = fieldRepository ?? throw new ArgumentNullException("FieldRepository");
        }

        public async Task Create(Product product, Field field, CancellationToken cancellationToken = default)
        {
            await Name(field, cancellationToken);
            await Value(field, cancellationToken);
        }

        public async Task Update(Product product, Field field, CancellationToken cancellationToken = default)
        {
            await Name(field, cancellationToken);
            await Value(field, cancellationToken);
        }

        public async Task Delete(Product product, Field field, CancellationToken cancellationToken = default)
        {
            if (!(await Exist(product, field, cancellationToken)))
                throw new Exception($"No existe el campo {product.Name}");
        }

        public async Task<bool> Exist(Product product, Field field, CancellationToken cancellationToken = default)
        {
            return (await _fieldRepository.Get(product, field.Id, cancellationToken)) != null;
        }

        public async Task Name(Field field, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(field.Name))
                throw new Exception("Debe ingresar un nombre.");
        }

        public async Task Value(Field field, CancellationToken cancellationToken = default)
        {
        }
    }
}
