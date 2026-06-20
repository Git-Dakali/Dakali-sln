using DK.Domain.Products;
using DK.Repositories.Products;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Validator.Products
{
    public class ProductColorValidator
    {
        public ProductColorRepository _productColorRepository;

        public ProductColorValidator(ProductColorRepository productColorRepository)
        {
            _productColorRepository = productColorRepository ?? throw new ArgumentNullException("ProductColorRepository");
        }

        public async Task Create(Product product, ProductColor color, CancellationToken cancellationToken = default)
        {
            await Name(color, cancellationToken);
            await Hex(color, cancellationToken);
        }

        public async Task Update(Product product, ProductColor color, CancellationToken cancellationToken = default)
        {
            await Name(color, cancellationToken);
            await Hex(color, cancellationToken);
        }

        public async Task Delete(Product product, ProductColor color, CancellationToken cancellationToken = default)
        {
            if (!await Exist(product, color, cancellationToken))
                throw new Exception($"No existe el color {color.Name}");
        }

        public async Task<bool> Exist(Product product, ProductColor color, CancellationToken cancellationToken = default)
        {
            return await _productColorRepository.Get(product, color.Id, cancellationToken) != null;
        }

        public async Task Name(ProductColor color, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(color.Name))
                throw new Exception("El nombre color esta vacio.");
        }

        public async Task Hex(ProductColor color, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace( color.Hex))
                throw new Exception("Debe seleccionar un color.");
        }
    }
}
