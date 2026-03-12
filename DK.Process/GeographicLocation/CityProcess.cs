using DK.Domain.GeographicLocation;
using DK.Repositories.GeographicLocation;
using DK.Validator.GeographicLocation;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Process.GeographicLocation
{
    public class CityProcess
    {
        private CityRepository _cityRepository;
        private CityValidator _cityValidator;

        public CityProcess(CityRepository cityRepository, CityValidator cityValidator)
        {
            _cityRepository = cityRepository;
            _cityValidator = cityValidator;
        }

        public async Task<IEnumerable<City>> GetAll(CancellationToken cancellationToken = default)
        {
            return await _cityRepository.GetAll(cancellationToken);
        }

        public async Task<City> Get(long id, CancellationToken cancellationToken = default)
        {
            return await _cityRepository.Get(id, cancellationToken);
        }

        public async Task<City> Get(string zipCode, CancellationToken cancellationToken = default)
        {
            return await _cityRepository.Get(zipCode, cancellationToken);
        }

        public async Task<IEnumerable<City>> Get(Province province, CancellationToken cancellationToken = default)
        {
            return await _cityRepository.Get(province, cancellationToken);
        }

        public async Task<City> Create(City entity, CancellationToken cancellationToken = default)
        {
            await _cityValidator.Create(entity, cancellationToken);
            return await _cityRepository.Create(entity, cancellationToken);
        }

        public async Task<City> Update(City entity, CancellationToken cancellationToken = default)
        {
            await _cityValidator.Update(entity, cancellationToken);
            return await _cityRepository.Update(entity, cancellationToken);
        }

        public async Task Delete(City entity, CancellationToken cancellationToken = default)
        {
            await _cityValidator.Delete(entity, cancellationToken);
            await _cityRepository.Delete(entity, cancellationToken);
        }
    }
}
