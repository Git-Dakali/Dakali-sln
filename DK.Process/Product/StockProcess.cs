using DK.Domain.Locations;
using DK.Domain.Products;
using DK.Domain.Sales;
using DK.Repositories.Products;
using DK.Validator;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task Recount(List<Stock> stocks, CancellationToken cancellationToken = default)
        {
            await _stockValidator.Recount(stocks, cancellationToken);
            var stocksPersisted = await _stockRepository.Get(stocks.First().Location, cancellationToken);
            
            foreach (var stock in stocks)
            {
                var stockPersisted = stocksPersisted.FirstOrDefault(x => x.ProductSku.Product.Id == stock.ProductSku.Product.Id && x.ProductSku.Variant.Id == stock.ProductSku.Variant.Id && x.ProductSku.Color.Id == stock.ProductSku.Color.Id);

                if (stockPersisted is null)
                    await _stockRepository.Create(stock, cancellationToken);
                else
                {
                    stockPersisted.Physical = stock.Physical;
                    await UpdatePhysical(stockPersisted, cancellationToken);
                }
            }

            foreach (var stockPersisted in stocksPersisted)
            {
                var stock = stocks.FirstOrDefault(x => x.ProductSku.Product.Id == stockPersisted.ProductSku.Product.Id && x.ProductSku.Color.Id == stockPersisted.ProductSku.Color.Id && x.ProductSku.Variant.Id == stockPersisted.ProductSku.Variant.Id);

                if (stock is null)
                    await _stockRepository.Delete(stockPersisted, cancellationToken);
            }
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

        public async Task UpdatePhysical(Stock stock, CancellationToken cancellationToken = default)
        {
            if (stock.Physical < 0)
                throw new Exception("El stock fisico no puede ser un valor negativo.");

            await _stockRepository.UpdatePhysical(stock, cancellationToken);
        }

        public async Task<Stock> Reserve(LocationState state, DK.Domain.Products.Product product, ProductSku productSku, long freeCount, CancellationToken cancellation = default)
        {
            await _stockValidator.Reserve(state, product, productSku, freeCount, cancellation);

            var stock = await _stockRepository.Get(state, productSku, freeCount, cancellation);

            if (stock is null)
                throw new Exception($"No existe un stock libre para el producto {product.Name}-{productSku.Variant.Name}-{productSku.Color.Name} con cantidad {freeCount} en estado {state.Name}");

            await _stockRepository.Reserved(stock, freeCount, cancellation);

            return await _stockRepository.Get(stock.Id, cancellation);
        }

        public async Task<Stock> Commit(Stock stock, int count, CancellationToken cancellationToken = default)
        {
            if (count < 1)
                throw new Exception($"No posee una cantidad el {stock.ProductSku.Product.Name} - {stock.ProductSku.Variant.Name} - {stock.ProductSku.Color.Name}");

            return await _stockRepository.Commit(stock, count, cancellationToken);
        }

        public async Task<Stock> CancelReserve(Stock stock, long count, CancellationToken cancellation = default)
        {
            await _stockValidator.CancelReserved(stock, count, cancellation);
            await _stockRepository.CancelReserved(stock, count, cancellation);

            return await _stockRepository.Get(stock.Id, cancellation);
        }
    }
}
