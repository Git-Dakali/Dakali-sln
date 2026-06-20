using DK.Domain.Products;
using DK.Repositories.Products;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Process.Product
{
    public class ProductSkuProcess
    {
        private ProductSkuRepository _productSkuRepository;

        public ProductSkuProcess(ProductSkuRepository productSkuRepository)
        {
            _productSkuRepository = productSkuRepository;
        }

        public async Task<ProductSku> GetBySku(string sku, CancellationToken cancellationToken = default)
        {
            return await _productSkuRepository.GetBySku(sku, cancellationToken);
        }
    }
}
