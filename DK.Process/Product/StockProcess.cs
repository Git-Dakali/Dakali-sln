using DK.Domain.Products;
using DK.Repositories.Products;
using DK.Validator;
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
    }
}
