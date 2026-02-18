using DK.Domain.Sales;
using DK.Repositories.Sales;
using DK.Validator.Sales;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Process.Sales
{
    public class TaxStatusProcess
    {
        private TaxStatusRepository _taxStatusRepository;
        private TaxStatusValidator _taxStatusValidator;

        public TaxStatusProcess(TaxStatusRepository taxStatusRepository, TaxStatusValidator taxStatusValidator)
        {
            _taxStatusRepository = taxStatusRepository;
            _taxStatusValidator = taxStatusValidator;
        }

        public async Task<IEnumerable<TaxStatus>> GetAll(CancellationToken cancellationToken = default)
        {
            return await _taxStatusRepository.GetAll(cancellationToken);
        }

        public async Task<TaxStatus> Get(long id, CancellationToken cancellationToken = default)
        {
            return await _taxStatusRepository.Get(id, cancellationToken);
        }

        public async Task<TaxStatus> Get(string code, CancellationToken cancellationToken = default)
        {
            return await _taxStatusRepository.Get(code, cancellationToken);
        }

        public async Task<TaxStatus> Create(TaxStatus entity, CancellationToken cancellationToken = default)
        {

            await _taxStatusValidator.Create(entity, cancellationToken);

            return await _taxStatusRepository.Create(entity, cancellationToken);
        }

        public async Task<TaxStatus> Update(TaxStatus entity, CancellationToken cancellationToken = default)
        {
            await _taxStatusValidator.Update(entity, cancellationToken);

            return await _taxStatusRepository.Update(entity, cancellationToken);
        }

        public async Task Delete(TaxStatus entity, CancellationToken cancellationToken = default)
        {
            await _taxStatusValidator.Delete(entity, cancellationToken);
            await _taxStatusRepository.Delete(entity, cancellationToken);
        }
    }
}
