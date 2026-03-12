using DK.Domain.RoadMaps;
using DK.Repositories.RoadMaps;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Validator.RoadMaps
{
    public class DriverValidator
    {
        private DriverRepository _driverRepository;

        public DriverValidator(DriverRepository driverRepository)
        {
            _driverRepository = driverRepository ?? throw new ArgumentNullException("DriverRepository");
        }

        public async Task Create(Driver entity, CancellationToken cancellationToken = default)
        {
            await FirstName(entity, cancellationToken);
            await LastName(entity, cancellationToken);
            await Dni(entity, cancellationToken);
        }

        public async Task Update(Driver entity, CancellationToken cancellationToken = default)
        {
            if (!await Exist(entity, cancellationToken))
                throw new Exception($"No existe el Chofer {entity.FirstName} {entity.LastName}");

            await FirstName(entity, cancellationToken);
            await LastName(entity, cancellationToken);
            await Dni(entity, cancellationToken);
        }

        public async Task Delete(Driver entity, CancellationToken cancellationToken = default)
        {
            if (!await Exist(entity, cancellationToken))
                throw new Exception($"No existe el Chofer {entity.FirstName} {entity.LastName}");
        }

        public async Task<bool> Exist(Driver entity, CancellationToken cancellationToken = default)
        {
            return await _driverRepository.Get(entity.Id, cancellationToken) != null;
        }

        public async Task FirstName(Driver entity, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(entity.FirstName))
                throw new Exception("El nombre esta vacío.");
        }

        public async Task LastName(Driver entity, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(entity.LastName))
                throw new Exception("El apellido esta vacío.");
        }

        public async Task Dni(Driver entity, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(entity.Dni))
                throw new Exception("El DNI esta vacío.");
        }
    }
}
