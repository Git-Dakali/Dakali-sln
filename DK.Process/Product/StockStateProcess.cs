using DK.Domain.Products;
using DK.Repositories.Products;
using DK.Validator;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Process.Product
{
    public class StockStateProcess
    {
        private StockStateRepository _stockStateRepository;
        private StockStateValidator _stockStateValidator;

        public StockStateProcess(StockStateRepository stockStateRepository, StockStateValidator stockStateValidator)
        {
            _stockStateRepository = stockStateRepository;
            _stockStateValidator = stockStateValidator;
        }

        public async Task<IEnumerable<StockState>> GetAll(CancellationToken cancellationToken = default)
        {
            return await _stockStateRepository.GetAll(cancellationToken);
        }

        public async Task<StockState> Get(long id, CancellationToken cancellationToken = default)
        {
            return await _stockStateRepository.Get(id, cancellationToken);
        }

        public async Task<StockState> Get(string code, CancellationToken cancellationToken = default)
        {
            return await _stockStateRepository.Get(code, cancellationToken);
        }

        public async Task<StockState> Create(StockState category, CancellationToken cancellationToken = default)
        {
            
            await _stockStateValidator.Create(category, cancellationToken);

            return await _stockStateRepository.Create(category, cancellationToken);
        }

        public async Task<StockState> Update(StockState category, CancellationToken cancellationToken = default)
        {
            await _stockStateValidator.Update(category, cancellationToken);

            return await _stockStateRepository.Update(category, cancellationToken);
        }

        public async Task Delete(StockState category, CancellationToken cancellationToken = default)
        {
            await _stockStateValidator.Delete(category, cancellationToken);
            await _stockStateRepository.Delete(category, cancellationToken);
        }
    }
}
