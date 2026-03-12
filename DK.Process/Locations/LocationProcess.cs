using DK.Domain.Locations;
using DK.Repositories.Locations;
using DK.Validator;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Process.Locations
{
    public class LocationProcess
    {
        private LocationRepository _locationRepository;
        private LocationValidator _locationValidator;

        public LocationProcess(LocationRepository stockRepository, LocationValidator stockValidator)
        {
            _locationRepository = stockRepository;
            _locationValidator = stockValidator;
        }

        public async Task<IEnumerable<Location>> GetAll(CancellationToken cancellationToken = default)
        {
            return await _locationRepository.GetAll(cancellationToken);
        }

        public async Task<Location> Get(long id, CancellationToken cancellationToken = default)
        {
            return await _locationRepository.Get(id, cancellationToken);
        }

        public async Task<Location> Get(string hallwayCode, string columnCode, string levelCode, CancellationToken cancellationToken = default)
        {
            return await _locationRepository.Get(hallwayCode, columnCode, levelCode, cancellationToken);
        }

        public async Task<Location> Create(Location entity, CancellationToken cancellationToken = default)
        {
            await _locationValidator.Create(entity, cancellationToken);
            return await _locationRepository.Create(entity, cancellationToken);
        }

        public async Task<Location> Update(Location entity, CancellationToken cancellationToken = default)
        {
            await _locationValidator.Update(entity, cancellationToken);
            return await _locationRepository.Update(entity, cancellationToken);
        }

        public async Task Delete(Location entity, CancellationToken cancellationToken = default)
        {
            await _locationValidator.Delete(entity, cancellationToken);
            await _locationRepository.Delete(entity, cancellationToken);
        }
    }
}
