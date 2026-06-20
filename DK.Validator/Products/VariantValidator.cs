using DK.Domain.Products;
using DK.Repositories.Products;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Validator.Products
{
    public class VariantValidator
    {
        public VariantRepository _variantRepository;
        public ProductColorValidator _productColorValidator;

        public VariantValidator(VariantRepository variantRepository, ProductColorValidator productColorValidator)
        {
            _variantRepository = variantRepository ?? throw new ArgumentNullException("VariantRepository");
            _productColorValidator = productColorValidator ?? throw new ArgumentNullException("ProductColorValidator");
        }

        public async Task Create(Product product, Variant variant, CancellationToken cancellationToken = default)
        {
            await Name(variant, cancellationToken);
        }

        public async Task Update(Product product, Variant variant, CancellationToken cancellationToken = default)
        {
            await Name(variant, cancellationToken);
        }

        public async Task Delete(Product product, Variant variant, CancellationToken cancellationToken = default)
        {
            if (!await Exist(product, variant, cancellationToken))
                throw new Exception($"No existe la variante de tamaño {variant.Name}");
        }

        public async Task<bool> Exist(Product product, Variant variant, CancellationToken cancellationToken = default)
        {
            return await _variantRepository.Get(product, variant.Id, cancellationToken) != null;
        }

        public async Task Name(Variant variant, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(variant.Name))
                throw new Exception($"El Nombre de la variante esta vacio.");
        }
    }
}
