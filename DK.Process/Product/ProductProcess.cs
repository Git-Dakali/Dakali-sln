using Dakali.Domine;
using DK.Domain.Products;
using DK.Repositories.Products;
using DK.Validator.Products;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Process.Product
{
    public class ProductProcess
    {
        private ProductRepository _productRepository;
        private ProductValidator _productValidator;

        public ProductProcess(ProductRepository productRepository, ProductValidator productValidator)
        {
            _productRepository = productRepository;
            _productValidator = productValidator;
        }

        public async Task<IEnumerable<Domain.Products.Product>> GetAll(CancellationToken cancellationToken = default)
        {
            return await _productRepository.GetAll(cancellationToken);
        }

        public async Task<ResultPage<Domain.Products.Product>> GetPage(ProductFilter productFilter, CancellationToken cancellationToken = default)
        {
            return await _productRepository.GetPage(productFilter, cancellationToken);
        }

        public async Task<Domain.Products.Product> Get(long id, CancellationToken cancellationToken = default)
        {
            return await _productRepository.Get(id, cancellationToken);
        }

        public async Task<Domain.Products.Product> Create(Domain.Products.Product product, CancellationToken cancellationToken = default)
        {
            await _productValidator.Create(product, cancellationToken);

            return await _productRepository.Create(product, cancellationToken);
        }

        public async Task<Domain.Products.Product> Update(Domain.Products.Product product, CancellationToken cancellationToken = default)
        {
            await _productValidator.Update(product, cancellationToken);

            return await _productRepository.Update(product, cancellationToken);
        }

        public async Task Delete(Domain.Products.Product product, CancellationToken cancellationToken = default)
        {
            await _productValidator.Delete(product, cancellationToken);
            await _productRepository.Delete(product, cancellationToken);
        }
    }
}
