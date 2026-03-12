using DK.Domain.Locations;
using DK.Domain.Sales;
using DK.Process.Locations;
using DK.Process.Product;
using DK.Repositories.Sales;
using DK.Validator.Sales;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Process.Sales
{
    public class SaleProcess
    {
        private SaleRepository _saleRepository;
        private SaleValidator _saleValidator;
        private SaleDetailProcess _saleDetailProcess;
        private StockProcess _stockProcess;
        private LocationStateProcess _locationStateProcess;

        public SaleProcess(SaleRepository saleRepository, SaleValidator saleValidator, SaleDetailProcess saleDetailProcess, StockProcess stockProcess, LocationStateProcess locationStateProcess)
        {
            _saleRepository = saleRepository;
            _saleValidator = saleValidator;
            _saleDetailProcess = saleDetailProcess;
            _stockProcess = stockProcess;
            _locationStateProcess = locationStateProcess;
        }

        public async Task<IEnumerable<Sale>> GetAll(CancellationToken cancellationToken = default)
        {
            return await _saleRepository.GetAll(cancellationToken);
        }

        public async Task<Sale> Get(long id, CancellationToken cancellationToken = default)
        {
            return await _saleRepository.Get(id, cancellationToken);
        }

        public async Task<Sale> GetByNumber(long number, CancellationToken cancellationToken = default)
        {
            return await _saleRepository.GetByNumber(number, cancellationToken);
        }

        public async Task<Sale> Create(Sale sale, CancellationToken cancellationToken = default)
        {
            await _saleValidator.Create(sale, cancellationToken);

            var saleResult = await _saleRepository.Create(sale, cancellationToken);
            var state = await _locationStateProcess.Get("DIS", cancellationToken);
            
            foreach (var detail in saleResult.SaleDetails) 
                await Reserve(state, saleResult, detail, cancellationToken);

            return await _saleRepository.Get(saleResult.Id, cancellationToken);
        }

        public async Task<Sale> Update(Sale product, CancellationToken cancellationToken = default)
        {
            await _saleValidator.Update(product, cancellationToken);

            return await _saleRepository.Update(product, cancellationToken);
        }

        public async Task Delete(Sale product, CancellationToken cancellationToken = default)
        {
            await _saleValidator.Delete(product, cancellationToken);
            await _saleRepository.Delete(product, cancellationToken);
        }

        private async Task Reserve(LocationState state, Sale sale, SaleDetail detail, CancellationToken cancellation)
        {
            var stock = await _stockProcess.Reserve(state, detail.Product, detail.Variant, detail.Color, detail.Count, cancellation);
            await _saleDetailProcess.AssignStock(sale, detail, stock, cancellation);
        }
    }
}
