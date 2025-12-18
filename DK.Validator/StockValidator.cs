using DK.Domain.Products;
using DK.Repositories.Products;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Validator
{
    public class StockValidator
    {
        public StockRepository _stockRepository;
        public ProductRepository _productRepository;

        public StockValidator(StockRepository stockRepository, ProductRepository productRepository)
        {
            _stockRepository = stockRepository ?? throw new ArgumentNullException("StockRepository");
            _productRepository = productRepository ?? throw new ArgumentNullException("ProductRepository");
        }

        public async Task Create(Stock stock, CancellationToken cancellationToken = default)
        {
            await Product(stock, cancellationToken);
            await Variant(stock, cancellationToken);
            await Color(stock, cancellationToken);
            await Physical(stock, cancellationToken);
            await Reserved(stock, cancellationToken);
            await Free(stock, cancellationToken);
            await Minimum(stock, cancellationToken);
            await Maximum(stock, cancellationToken);
            await Status(stock, cancellationToken);
        }

        public async Task Update(Stock stock, CancellationToken cancellationToken = default)
        {
            if (!(await Exist(stock, cancellationToken)))
                throw new Exception($"No existe el stock {stock.Product.Name}-{stock.Variant.Name}-{stock.Color.Name}");

            await Product(stock, cancellationToken);
            await Variant(stock, cancellationToken);
            await Color(stock, cancellationToken);
            await Physical(stock, cancellationToken);
            await Reserved(stock, cancellationToken);
            await Free(stock, cancellationToken);
            await Minimum(stock, cancellationToken);
            await Maximum(stock, cancellationToken);
            await Status(stock, cancellationToken);
        }

        public async Task Delete(Stock stock, CancellationToken cancellationToken = default)
        {
            if (!(await Exist(stock, cancellationToken)))
                throw new Exception($"No existe el stock {stock.Product.Name}-{stock.Variant.Name}-{stock.Color.Name}");

        }

        public async Task<bool> Exist(Stock stock, CancellationToken cancellationToken = default)
        {
            return (await _stockRepository.Get(stock.Id, cancellationToken)) != null;
        }

        public async Task Product(Stock stock, CancellationToken cancellationToken = default)
        {
            if (stock.Product is null)
                throw new Exception("Producto vacio.");

            var productPersisted = await _productRepository.Get(stock.Product.Id, cancellationToken);

            if (productPersisted is null)
                throw new Exception($"El producto {stock.Product.Name} no existe.");
        }

        public async Task Variant(Stock stock, CancellationToken cancellationToken = default)
        {
            if (stock.Variant is null)
                throw new Exception("Variante vacio.");

            if (stock.Product is null)
                return;

            var productPersisted = await _productRepository.Get(stock.Product.Id, cancellationToken);

            if (productPersisted is null)
                return;

            if (!productPersisted.Variants.Any(v => v.Id == stock.Variant.Id))
                throw new Exception($"La variante {stock.Variant.Name} no existe para el producto {stock.Product.Name}");
        }

        public async Task Color(Stock stock, CancellationToken cancellationToken = default)
        {
            if (stock.Color is null)
                throw new Exception("El color vacio.");

            if (stock.Product is null || stock.Variant is null)
                return;

            var productPersisted = await _productRepository.Get(stock.Product.Id, cancellationToken);

            if (productPersisted is null)
                return;

            var variant = productPersisted.Variants.SingleOrDefault(v => v.Id == stock.Variant.Id);

            if (variant is null)
                return;

            if (!variant.ColorsHex.Any(c => c.Id == stock.Color.Id))
                throw new Exception($"La variante {stock.Variant.Name} no existe para el producto {stock.Product.Name}");
        }

        public async Task Physical(Stock stock, CancellationToken cancellationToken = default)
        {
            if (stock.Physical < 0)
                throw new Exception("El stock fisico, no puede ser un valor negativo");
        }

        public async Task Reserved(Stock stock, CancellationToken cancellationToken = default)
        {
            if (stock.Reserved < 0)
                throw new Exception("El stock reservado, no puede ser un valor negativo");

        }

        public async Task Free(Stock stock, CancellationToken cancellationToken = default)
        {
            if (stock.Free < 0)
                throw new Exception("El stock libre, no puede ser un valor negativo");
        }

        public async Task Minimum(Stock stock, CancellationToken cancellationToken = default)
        {
            if (stock.Minimum < 0)
                throw new Exception("El stock minimo, no puede ser un valor negativo");
        }

        public async Task Maximum(Stock stock, CancellationToken cancellationToken = default)
        {
            if (stock.Maximum < 0)
                throw new Exception("El stock maximo, no puede ser un valor negativo");
        }

        public async Task Status(Stock stock, CancellationToken cancellationToken = default)
        {
        }
    }
}
