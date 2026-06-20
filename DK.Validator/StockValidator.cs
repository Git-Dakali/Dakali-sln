using DK.Domain.Locations;
using DK.Domain.Products;
using DK.Repositories.Locations;
using DK.Repositories.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Validator
{
    public class StockValidator
    {
        public StockRepository _stockRepository;
        public ProductRepository _productRepository;
        public LocationRepository _locationRepository;

        public StockValidator(StockRepository stockRepository, ProductRepository productRepository, LocationRepository locationRepository)
        {
            _stockRepository = stockRepository ?? throw new ArgumentNullException("StockRepository");
            _productRepository = productRepository ?? throw new ArgumentNullException("ProductRepository");
            _locationRepository = locationRepository ?? throw new ArgumentNullException("LocationRepository");
        }

        public async Task Create(Stock stock, CancellationToken cancellationToken = default)
        {
            await Product(stock, cancellationToken);
            await ProductSku(stock, cancellationToken);
            await Physical(stock, cancellationToken);
            await Reserved(stock, cancellationToken);
            await Free(stock, cancellationToken);
            await Minimum(stock, cancellationToken);
            await Maximum(stock, cancellationToken);
            await Location(stock, cancellationToken);

            var stockPersisted = await _stockRepository.Get(stock.ProductSku, stock.Location, cancellationToken);

            if (stockPersisted is null)
                return;

            throw new Exception($"El Stock {stock.ProductSku.Product.Name}-{stock.ProductSku.Variant.Name}-{stock.ProductSku.Color.Name} ya existe para la ubicacion {stock.Location.Hallway.Name}-{stock.Location.Column.Name}-{stock.Location.Level.Name}");
        }

        public async Task Recount(List<Stock> stocks, CancellationToken cancellationToken = default)
        {
            var firstStock = stocks.First();
            if (stocks.Any(s => s.Location.Id != firstStock.Location.Id))
                throw new Exception("Existe ubicaciones distintas.");

            foreach (var stock in stocks)
            {
                await Product(stock, cancellationToken);
                await ProductSku(stock, cancellationToken);
                await Physical(stock, cancellationToken);
                await Reserved(stock, cancellationToken);
                await Free(stock, cancellationToken);
                await Minimum(stock, cancellationToken);
                await Maximum(stock, cancellationToken);
                await Location(stock, cancellationToken);

                var count = stocks.Count(x => x.ProductSku.Id == stock.ProductSku.Id && x.Location.Id == stock.Location.Id);

                if (count > 1)
                    throw new Exception($"El Stock {stock.ProductSku.Product.Name}-{stock.ProductSku.Variant.Name}-{stock.ProductSku.Color.Name} ya existe para la ubicacion {stock.Location.Hallway.Name}-{stock.Location.Column.Name}-{stock.Location.Level.Name}");        
            }
        }

        public async Task StockEntry(Stock stock, int amount, CancellationToken cancellationToken = default)
        {
            if (!(await Exist(stock, cancellationToken)))
                throw new Exception($"No existe el stock {stock.ProductSku.Product.Name}-{stock.ProductSku.Variant.Name}-{stock.ProductSku.Color.Name}");

            if (amount <= 0)
                throw new Exception($"De ingresar una cantidad.");
        }

        public async Task Delete(Stock stock, CancellationToken cancellationToken = default)
        {
            if (!(await Exist(stock, cancellationToken)))
                throw new Exception($"No existe el stock {stock.ProductSku.Product.Name}-{stock.ProductSku.Variant.Name}-{stock.ProductSku.Color.Name}");

        }

        public async Task<bool> Exist(Stock stock, CancellationToken cancellationToken = default)
        {
            return (await _stockRepository.Get(stock.Id, cancellationToken)) != null;
        }

        public async Task Product(Stock stock, CancellationToken cancellationToken = default)
        {
            if (stock.ProductSku.Product is null)
                throw new Exception("Producto vacio.");

            var productPersisted = await _productRepository.Get(stock.ProductSku.Product.Id, cancellationToken);

            if (productPersisted is null)
                throw new Exception($"El producto {stock.ProductSku.Product.Name} no existe.");
        }

        public async Task ProductSku(Stock stock, CancellationToken cancellationToken = default)
        {
            if (stock.ProductSku is null)
                throw new Exception("Debe cargar un producto.");
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

        public async Task Location(Stock stock, CancellationToken cancellationToken = default)
        {
            if (stock.Location is null)
                throw new Exception("La ubicacion esta vacio.");

            var locationPersisted = await _locationRepository.Get(stock.Location.Id, cancellationToken);

            if (locationPersisted is null)
                throw new Exception($"La ubicacion {stock.Location.Hallway?.Name ?? string.Empty}-{stock.Location.Column?.Name ?? string.Empty}-{stock.Location.Level?.Name ?? string.Empty} no existe.");

        }

        public async Task Reserve(LocationState state, Product product, ProductSku productSku, long freeCount, CancellationToken cancellation = default)
        {
            if (state == null)
                throw new Exception("El estado de la ubicacion esta vacio.");
            if (product == null)
                throw new Exception("El producto esta vacio.");
            if (productSku == null)
                throw new Exception("El producto esta vacio.");
            if (freeCount <= 0)
                throw new Exception("La cantidad a reservar del stock debe ser mayor a cero");

            var stock = await _stockRepository.Get(state, productSku, freeCount, cancellation);

            if (stock is null)
                throw new Exception($"No existe un stock libre para el producto {product.Name}-{productSku.Variant.Name}-{productSku.Color.Name} con cantidad {freeCount} en estado {state.Name}");
        }

        public async Task CancelReserved(Stock stock, long count, CancellationToken cancellation = default)
        {
            if(stock.Id == 0 )
                throw new Exception("No existe el Stock");

            if(count <= 0)
                throw new Exception("La cantidad a eliminar la reserva de stock debe ser mayor a cero");

            var stockPersisted = await _stockRepository.Get(stock.Id, cancellation);

            if (stockPersisted is null)
                throw new Exception($"No existe un stock para el producto {stock.ProductSku.Product?.Name}-{stock.ProductSku.Variant?.Name}-{stock.ProductSku.Color?.Name} con cantidad {count} en estado {stock.Location?.State?.Name}");
        }
    }
}
