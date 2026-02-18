using DK.Domain.Locations;
using DK.Domain.Products;
using DK.Domain.Sales;
using DK.Repositories.Products;
using DK.Validator;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Process.Product
{
    public class StockProcess
    {
        private StockRepository _stockRepository;
        private StockValidator _stockValidator;

        public StockProcess(StockRepository stockRepository, StockValidator stockValidator)
        {
            _stockRepository = stockRepository;
            _stockValidator = stockValidator;
        }

        public async Task<IEnumerable<Stock>> GetAll(CancellationToken cancellationToken = default)
        {
            return await _stockRepository.GetAll(cancellationToken);
        }

        public async Task<IEnumerable<Stock>> GetAll(string searchString, CancellationToken cancellationToken = default)
        {
            return await _stockRepository.GetAll(searchString, cancellationToken);
        }

        public async Task<Stock> Get(long id, CancellationToken cancellationToken = default)
        {
            return await _stockRepository.Get(id, cancellationToken);
        }

        public async Task<Stock> Create(Stock stock, CancellationToken cancellationToken = default)
        {
            
            await _stockValidator.Create(stock, cancellationToken);

            return await _stockRepository.Create(stock, cancellationToken);
        }

        public async Task StockEntry(Stock stock, int amount, CancellationToken cancellationToken = default)
        {
            await _stockValidator.StockEntry(stock, amount, cancellationToken);
            await _stockRepository.StockEntry(stock, amount, cancellationToken);
        }

        public async Task Delete(Stock stock, CancellationToken cancellationToken = default)
        {
            await _stockValidator.Delete(stock, cancellationToken);

            await _stockRepository.Delete(stock, cancellationToken);
        }

        public async Task<Stock> Reserve(LocationState state, DK.Domain.Products.Product product, Variant variant, ProductColor color, long freeCount, CancellationToken cancellation = default)
        {
            await _stockValidator.Reserve(state, product, variant, color, freeCount, cancellation);

            var stock = await _stockRepository.Get(state, product, variant, color, freeCount, cancellation);

            if (stock is null)
                throw new Exception($"No existe un stock libre para el producto {product.Name}-{variant.Name}-{color.Name} con cantidad {freeCount} en estado {state.Name}");

            await _stockRepository.Reserved(stock, freeCount, cancellation);

            return await _stockRepository.Get(stock.Id, cancellation);
        }

        public async Task<Stock> CancelReserve(Stock stock, long count, CancellationToken cancellation = default)
        {
            await _stockValidator.CancelReserved(stock, count, cancellation);
            await _stockRepository.CancelReserved(stock, count, cancellation);

            return await _stockRepository.Get(stock.Id, cancellation);
        }
    }
}
