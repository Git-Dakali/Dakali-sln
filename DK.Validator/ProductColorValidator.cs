using DK.Domain.Products;
using DK.Repositories.Products;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Validator
{
    public class ProductColorValidator
    {
        public ProductColorRepository _productColorRepository;

        public ProductColorValidator(ProductColorRepository productColorRepository)
        {
            _productColorRepository = productColorRepository ?? throw new ArgumentNullException("ProductColorRepository");
        }

        public async Task Create(Variant variant, ProductColor color, CancellationToken cancellationToken = default)
        {
            await Name(color, cancellationToken);
            await Hex(color, cancellationToken);
            await Sku(color, cancellationToken);

            var colorPersister = await _productColorRepository.Get(color.Sku, cancellationToken);

            if (colorPersister != null)
                throw new Exception($"Ya existe en otro producto el SKU {color.Sku}");
        }

        public async Task Update(Variant variant, ProductColor color, CancellationToken cancellationToken = default)
        {
            await Name(color, cancellationToken);
            await Hex(color, cancellationToken);
        }

        public async Task Delete(Variant variant, ProductColor color, CancellationToken cancellationToken = default)
        {
            if (!(await Exist(variant, color, cancellationToken)))
                throw new Exception($"No existe el color {color.Name}");
        }

        public async Task<bool> Exist(Variant variant, ProductColor color, CancellationToken cancellationToken = default)
        {
            return (await _productColorRepository.Get(variant, color.Id, cancellationToken)) != null;
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

        public async Task Sku(ProductColor color, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(color.Sku))
                throw new Exception("Debe ingresar un SKU.");
        }
    }
}
