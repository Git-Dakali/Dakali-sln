using DK.Domain.Products;
using DK.Repositories.Products;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Validator.Products
{
    public class ProductValidator
    {
        public ProductRepository _productRepository;
        public VariantValidator _variantValidator;
        public FieldValidator _fieldValidator;
        public ProductColorValidator _productColorValidator;
        public ProductSkuValidator _productSkuValidator;

        public ProductValidator(ProductRepository productRepository, VariantValidator variantValidator, FieldValidator fieldValidator, ProductColorValidator productColorValidator, ProductSkuValidator productSkuValidator)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException("ProductRepository");
            _variantValidator = variantValidator ?? throw new ArgumentNullException("VariantValidator");
            _fieldValidator = fieldValidator ?? throw new ArgumentNullException("FieldValidator");
            _productColorValidator = productColorValidator ?? throw new ArgumentNullException("productColorValidator");
            _productSkuValidator = productSkuValidator ?? throw new ArgumentNullException("productSkuValidator");
        }

        public async Task Create(Product product, CancellationToken cancellationToken = default)
        {
            await Category(product, cancellationToken);
            await Code(product, cancellationToken);
            await Name(product, cancellationToken);
            await Price(product, cancellationToken);
            await SalePrice(product, cancellationToken);
            await Description(product, cancellationToken);
            await Variants(product, cancellationToken);
            await Fields(product, cancellationToken);
            await Colors(product, cancellationToken);
            await Skus(product, cancellationToken);

            foreach (var variant in product.Variants)
                await _variantValidator.Create(product, variant, cancellationToken);

            foreach (var field in product.Fields)
                await _fieldValidator.Create(product, field, cancellationToken);

            foreach (var color in product.Colors)
                await _productColorValidator.Create(product, color, cancellationToken);

            foreach (var sku in product.Skus)
                await _productSkuValidator.Create(product, sku, cancellationToken);
        }

        public async Task Update(Product product, CancellationToken cancellationToken = default)
        {
            await Category(product, cancellationToken);
            await Code(product, cancellationToken);
            await Name(product, cancellationToken);
            await Price(product, cancellationToken);
            await SalePrice(product, cancellationToken);
            await Description(product, cancellationToken);
            await Variants(product, cancellationToken);
            await Fields(product, cancellationToken);
            await Colors(product, cancellationToken);
            await Skus(product, cancellationToken);

            foreach (var variant in product.Variants)
                await _variantValidator.Update(product, variant, cancellationToken);

            foreach (var field in product.Fields)
                await _fieldValidator.Update(product, field, cancellationToken);

            foreach (var color in product.Colors)
                await _productColorValidator.Update(product, color, cancellationToken);

            foreach (var sku in product.Skus)
                await _productSkuValidator.Update(product, sku, cancellationToken);
        }

        public async Task Delete(Product product, CancellationToken cancellationToken = default)
        {
            if (!await Exist(product, cancellationToken))
                throw new Exception($"No existe el producto {product.Name}");

            foreach (var item in product.Variants)
                await _variantValidator.Delete(product, item, cancellationToken);
        }

        public async Task<bool> Exist(Product product, CancellationToken cancellationToken = default)
        {
            return await _productRepository.Get(product.Id, cancellationToken) != null;
        }

        public async Task Code(Product product, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(product.Code))
                throw new Exception("El codigo esta vacío.");

            if (product.Id > 0)
                return;

            var categoryPersisted = await _productRepository.Get(product.Code, cancellationToken);

            if (categoryPersisted != null)
                throw new Exception($"El codigo {product.Code} ya existe.");
        }

        public async Task Name(Product product, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(product.Name))
                throw new Exception("Debe ingresar un nombre.");
        }

        public async Task Description(Product product, CancellationToken cancellationToken = default)
        {
        }

        public async Task Active(Product product, CancellationToken cancellationToken = default)
        {
        }

        public async Task Price(Product product, CancellationToken cancellationToken = default)
        {
            if (product.Price <= 0)
                throw new Exception("Debe ingresar un precio.");
        }

        public async Task SalePrice(Product product, CancellationToken cancellationToken = default)
        {
            if (product.SalePrice <= 0)
                throw new Exception("Debe ingresar un precio de venta.");
        }

        public async Task Category(Product product, CancellationToken cancellationToken = default)
        {
            if (product.Category is null)
                throw new Exception("Debe ingresar una Categoria.");
        }

        public async Task Variants(Product product, CancellationToken cancellationToken = default)
        {
            if (product.Variants is null)
                throw new Exception("Debe ingresar variantes.");
            if (product.Variants.Count() == 0)
                throw new Exception("Debe ingresar variantes.");
        }

        public async Task Fields(Product product, CancellationToken cancellationToken = default)
        {

        }

        public async Task Colors(Product product, CancellationToken cancellationToken = default)
        {
            if (product.Colors is null)
                throw new Exception("Debe ingresar Colores.");
            if (product.Colors.Count() == 0)
                throw new Exception("Debe ingresar Colores.");
        }

        public async Task Skus(Product product, CancellationToken cancellationToken = default)
        {
            if (product.Skus is null)
                throw new Exception("Debe ingresar Skus.");
            if (product.Skus.Count() == 0)
                throw new Exception("Debe ingresar Skus.");

            if (product.Skus.Count() != product.Variants.Count() * product.Colors.Count())
                throw new Exception("Debe cargar todos los SKU para todas las variantes y colores.");
        }
    }
}
