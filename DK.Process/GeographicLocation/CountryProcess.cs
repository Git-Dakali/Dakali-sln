using DK.Domain.GeographicLocation;
using DK.Repositories.GeographicLocation;
using DK.Validator.GeographicLocation;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Process.GeographicLocation
{
    public class CountryProcess
    {
        private CountryRepository _countryRepository;
        private CountryValidator _countryValidator;

        public CountryProcess(CountryRepository countryRepository, CountryValidator countryValidator)
        {
            _countryRepository = countryRepository;
            _countryValidator = countryValidator;
        }

        public async Task<IEnumerable<Country>> GetAll(CancellationToken cancellationToken = default)
        {
            return await _countryRepository.GetAll(cancellationToken);
        }

        public async Task<Country> Get(long id, CancellationToken cancellationToken = default)
        {
            return await _countryRepository.Get(id, cancellationToken);
        }

        public async Task<Country> Get(string code, CancellationToken cancellationToken = default)
        {
            return await _countryRepository.Get(code, cancellationToken);
        }

        public async Task<Country> Create(Country category, CancellationToken cancellationToken = default)
        {
            
            await _countryValidator.Create(category, cancellationToken);

            return await _countryRepository.Create(category, cancellationToken);
        }

        public async Task<Country> Update(Country category, CancellationToken cancellationToken = default)
        {
            await _countryValidator.Update(category, cancellationToken);

            return await _countryRepository.Update(category, cancellationToken);
        }

        public async Task Delete(Country category, CancellationToken cancellationToken = default)
        {
            await _countryValidator.Delete(category, cancellationToken);
            await _countryRepository.Delete(category, cancellationToken);
        }
    }
}
