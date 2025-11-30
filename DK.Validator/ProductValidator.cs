using DK.Domain.Products;
using DK.Repositories.Products;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Validator
{
    public class ProductValidator
    {
        public ProductRepository _productRepository;
        public ModelValidator _modelValidator;

        public ProductValidator(ProductRepository productRepository, ModelValidator modelValidator)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException("ProductRepository");
            _modelValidator = modelValidator ?? throw new ArgumentNullException("ModelValidator");
        }

        public async Task Create(Product product, CancellationToken cancellationToken = default)
        {
            await Name(product, cancellationToken);
            await Description(product, cancellationToken);
            await Model(product, cancellationToken);
            await Variant(product, cancellationToken);
        }

        public async Task Update(Product product, CancellationToken cancellationToken = default)
        {
            await Name(product, cancellationToken);
            await Description(product, cancellationToken);
            await Model(product, cancellationToken);
            await Variant(product, cancellationToken);
        }

        public async Task Delete(Product product, CancellationToken cancellationToken = default)
        {
            if (!(await Exist(product, cancellationToken)))
                throw new Exception($"No existe el producto {product.Name}");
        }

        public async Task<bool> Exist(Product product, CancellationToken cancellationToken = default)
        {
            return (await _productRepository.Get(product.Id, cancellationToken)) != null;
        }

        public async Task Name(Product product, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(product.Name))
                throw new Exception("Debe ingresar un nombre.");
        }

        public async Task Description(Product product, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(product.Description))
                throw new Exception("Debe ingresar una descripción.");
        }

        public async Task Model(Product product, CancellationToken cancellationToken = default)
        {
            if (product.Model is null)
                throw new Exception("Debe ingresar un modelo.");

            var result = await _modelValidator.Exist(product.Model, cancellationToken);

            if (!result)
                throw new Exception("El modelo no existe.");
        }

        public async Task Variant(Product product, CancellationToken cancellationToken = default)
        {
            if (product.Variants is null)
                throw new Exception("Debe ingresar variantes.");
            if (product.Variants.Count() == 0)
                throw new Exception("Debe ingresar variantes.");

            if(product.Model.VariantNames.Count() != product.Variants.Count())
                throw new Exception("Las variantes del modelo, no estan todos configurado como variante en el producto.");

            foreach (var variantName in product.Model.VariantNames)
            {
                var variant = product.Variants.FirstOrDefault(v => v.Name.ToUpper() == variantName.ToUpper());

                if (variant is null)
                    throw new Exception($"La variante {variantName}, no existe como una variante en el Modelo.");
            }
        }
    }
}
