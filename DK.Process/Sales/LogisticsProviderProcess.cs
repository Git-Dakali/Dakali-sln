using DK.Domain.Sales;
using DK.Repositories.Sales;
using DK.Validator.Sales;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Process.Sales
{
    public class LogisticsProviderProcess
    {
        private LogisticsProviderRepository _logisticsProviderRepository;
        private LogisticsProviderValidator _logisticsProviderValidator;

        public LogisticsProviderProcess(LogisticsProviderRepository logisticsProviderRepository, LogisticsProviderValidator logisticsProviderValidator)
        {
            _logisticsProviderRepository = logisticsProviderRepository;
            _logisticsProviderValidator = logisticsProviderValidator;
        }

        public async Task<IEnumerable<LogisticsProvider>> GetAll(CancellationToken cancellationToken = default)
        {
            return await _logisticsProviderRepository.GetAll(cancellationToken);
        }

        public async Task<LogisticsProvider> Get(long id, CancellationToken cancellationToken = default)
        {
            return await _logisticsProviderRepository.Get(id, cancellationToken);
        }

        public async Task<LogisticsProvider> Get(string code, CancellationToken cancellationToken = default)
        {
            return await _logisticsProviderRepository.Get(code, cancellationToken);
        }

        public async Task<LogisticsProvider> Create(LogisticsProvider logisticsProvider, CancellationToken cancellationToken = default)
        {
            await _logisticsProviderValidator.Create(logisticsProvider, cancellationToken);
            return await _logisticsProviderRepository.Create(logisticsProvider, cancellationToken);
        }

        public async Task<LogisticsProvider> Update(LogisticsProvider logisticsProvider, CancellationToken cancellationToken = default)
        {
            await _logisticsProviderValidator.Update(logisticsProvider, cancellationToken);
            return await _logisticsProviderRepository.Update(logisticsProvider, cancellationToken);
        }

        public async Task Delete(LogisticsProvider logisticsProvider, CancellationToken cancellationToken = default)
        {
            await _logisticsProviderValidator.Delete(logisticsProvider, cancellationToken);
            await _logisticsProviderRepository.Delete(logisticsProvider, cancellationToken);
        }
    }
}
