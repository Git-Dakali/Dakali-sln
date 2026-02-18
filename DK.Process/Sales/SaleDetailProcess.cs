using DK.Domain.Products;
using DK.Domain.Sales;
using DK.Repositories.Sales;
using DK.Validator.Sales;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Process.Sales
{
    public class SaleDetailProcess
    {
        private SaleDetailRepository _saleDetailRepository;
        private SaleDetailValidator _saleDetailValidator;

        public SaleDetailProcess(SaleDetailRepository saleDetailRepository, SaleDetailValidator saleDetailValidator)
        {
            _saleDetailRepository = saleDetailRepository;
            _saleDetailValidator = saleDetailValidator;
        }

        public async Task<IEnumerable<SaleDetail>> Get(Sale sale, CancellationToken cancellation = default)
        {
            return await _saleDetailRepository.Get(sale, cancellation);
        }

        public async Task<SaleDetail> Get(Sale parent, long idSaleDetail, CancellationToken cancellation = default)
        {
            return await _saleDetailRepository.Get(parent, idSaleDetail, cancellation);
        }

        public async Task<SaleDetail> Create(Sale parent, SaleDetail saleDetail, CancellationToken cancellation = default)
        {
            await _saleDetailValidator.Create(parent, saleDetail, cancellation);
            return await _saleDetailRepository.Create(parent, saleDetail, cancellation);
        }

        public async Task Delete(Sale parent, SaleDetail saleDetail, CancellationToken cancellation = default)
        {
            await _saleDetailValidator.Delete(parent, saleDetail, cancellation);
            await _saleDetailRepository.Delete(parent, saleDetail, cancellation);
        }

        public async Task AssignStock(Sale parent, SaleDetail saleDetail, Stock stock, CancellationToken cancellation = default)
        {
            
            await _saleDetailRepository.AssignStock(parent, saleDetail, stock, cancellation);
        }

        public async Task UnassignStock(Sale parent, SaleDetail saleDetail, CancellationToken cancellation = default)
        {
            await _saleDetailRepository.UnassignStock(parent, saleDetail, cancellation);
        }
    }
}
