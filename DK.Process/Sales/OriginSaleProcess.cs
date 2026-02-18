using DK.Domain.Sales;
using DK.Repositories.Sales;
using DK.Validator.Sales;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Process.Sales
{
    public class OriginSaleProcess
    {
        private OriginSaleRepository _originSaleRepository;
        private OriginSaleValidator _originSaleValidator;

        public OriginSaleProcess(OriginSaleRepository originSaleRepository, OriginSaleValidator originSaleValidator)
        {
            _originSaleRepository = originSaleRepository;
            _originSaleValidator = originSaleValidator;
        }

        public async Task<IEnumerable<OriginSale>> GetAll(CancellationToken cancellationToken = default)
        {
            return await _originSaleRepository.GetAll(cancellationToken);
        }

        public async Task<OriginSale> Get(long id, CancellationToken cancellationToken = default)
        {
            return await _originSaleRepository.Get(id, cancellationToken);
        }

        public async Task<OriginSale> Get(string code, CancellationToken cancellationToken = default)
        {
            return await _originSaleRepository.Get(code, cancellationToken);
        }

        public async Task<OriginSale> Create(OriginSale originSale, CancellationToken cancellationToken = default)
        {
            await _originSaleValidator.Create(originSale, cancellationToken);
            return await _originSaleRepository.Create(originSale, cancellationToken);
        }

        public async Task<OriginSale> Update(OriginSale originSale, CancellationToken cancellationToken = default)
        {
            await _originSaleValidator.Update(originSale, cancellationToken);
            return await _originSaleRepository.Update(originSale, cancellationToken);
        }

        public async Task Delete(OriginSale originSale, CancellationToken cancellationToken = default)
        {
            await _originSaleValidator.Delete(originSale, cancellationToken);
            await _originSaleRepository.Delete(originSale, cancellationToken);
        }
    }
}
