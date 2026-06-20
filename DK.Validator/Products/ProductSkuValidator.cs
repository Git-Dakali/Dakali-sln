using DK.Domain.Products;
using DK.Repositories.Products;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Validator.Products
{
    public class ProductSkuValidator
    {
        public ProductSkuRepository _productSkuRepository;

        public ProductSkuValidator(ProductSkuRepository productSkuRepository)
        {
            _productSkuRepository = productSkuRepository ?? throw new ArgumentNullException("FieldRepository");
        }

        public async Task Create(Product product, ProductSku productSku, CancellationToken cancellationToken = default)
        {
            await Color(product, productSku, cancellationToken);
            await Variant(product, productSku, cancellationToken);
            await Sku(product, productSku, cancellationToken);
        }

        public async Task Update(Product product, ProductSku productSku, CancellationToken cancellationToken = default)
        {
            await Color(product, productSku, cancellationToken);
            await Variant(product, productSku, cancellationToken);
            await Sku(product, productSku, cancellationToken);
        }

        public async Task Delete(Product product, ProductSku productSku, CancellationToken cancellationToken = default)
        {
            if (!(await Exist(product, productSku, cancellationToken)))
                throw new Exception($"No existe el SKU {productSku.Sku}");
        }

        public async Task<bool> Exist(Product product, ProductSku productSku, CancellationToken cancellationToken = default)
        {
            return (await _productSkuRepository.Get(product, productSku.Id, cancellationToken)) != null;
        }

        public async Task Variant(Product product, ProductSku productSku, CancellationToken cancellationToken = default)
        {
            if (productSku.Variant is null)
                throw new Exception($"Debe ingresar una Variante para el SKU {productSku.Sku}");

            if (!product.Variants.Any(v => v.Id == productSku.Variant.Id))
                throw new Exception($"No existe la variante {productSku.Variant.Name} dentro del Producto.");
        }

        public async Task Color(Product product, ProductSku productSku, CancellationToken cancellationToken = default)
        {
            if (productSku.Color is null)
                throw new Exception($"Debe ingresar un color para el SKU {productSku.Sku}");

            if (!product.Colors.Any(v => v.Id == productSku.Color.Id))
                throw new Exception($"No existe el color {productSku.Color.Name} dentro del Producto.");
        }

        public async Task Sku(Product product, ProductSku productSku, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(productSku.Sku))
                throw new Exception("Debe ingresar un SKU");
        }
    }
}
