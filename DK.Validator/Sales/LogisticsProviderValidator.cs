using DK.Domain.Sales;
using DK.Repositories.Sales;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Validator.Sales
{
    public class LogisticsProviderValidator
    {
        private LogisticsProviderRepository _logisticsProviderRepository;

        public LogisticsProviderValidator(LogisticsProviderRepository logisticsProviderRepository)
        {
            _logisticsProviderRepository = logisticsProviderRepository ?? throw new ArgumentNullException("LogisticsProviderRepository");
        }

        public async Task Create(LogisticsProvider category, CancellationToken cancellationToken = default)
        {
            await Code(category, cancellationToken);
            await Name(category, cancellationToken);
        }

        public async Task Update(LogisticsProvider entity, CancellationToken cancellationToken = default)
        {
            if (!await Exist(entity, cancellationToken))
                throw new Exception($"No existe el Proveedor Logistico {entity.Name}");

            await Name(entity, cancellationToken);
        }

        public async Task Delete(LogisticsProvider entity, CancellationToken cancellationToken = default)
        {
            if (!await Exist(entity, cancellationToken))
                throw new Exception($"No existe el Proveedor Logistico {entity.Name}");
        }

        public async Task<bool> Exist(LogisticsProvider entity, CancellationToken cancellationToken = default)
        {
            return await _logisticsProviderRepository.Get(entity.Id, cancellationToken) != null;
        }

        public async Task Code(LogisticsProvider entity, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(entity.Code))
                throw new Exception("El codigo esta vacío.");

            if (entity.Id > 0)
                return;

            var persisted = await _logisticsProviderRepository.Get(entity.Code, cancellationToken);

            if (persisted != null)
                throw new Exception($"El codigo {entity.Code} ya existe.");
        }

        public async Task Name(LogisticsProvider entity, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(entity.Name))
                throw new Exception("El nombre esta vacío.");
        }
    }
}
