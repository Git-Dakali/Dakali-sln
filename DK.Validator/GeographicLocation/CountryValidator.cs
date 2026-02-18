using DK.Domain.GeographicLocation;
using DK.Repositories.GeographicLocation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Validator.GeographicLocation
{
    public class CountryValidator
    {
        private CountryRepository _countryRepository;

        public CountryValidator(CountryRepository countryRepository)
        {
            _countryRepository = countryRepository ?? throw new ArgumentNullException("CountryRepository");
        }

        public async Task Create(Country entity, CancellationToken cancellationToken = default)
        {
            await Code(entity, cancellationToken);
            await Name(entity, cancellationToken);
        }

        public async Task Update(Country entity, CancellationToken cancellationToken = default)
        {
            if (!(await Exist(entity, cancellationToken)))
                throw new Exception($"No existe el Pais {entity.Code}-{entity.Name}");

            await Code(entity, cancellationToken);
            await Name(entity, cancellationToken);
        }

        public async Task Delete(Country entity, CancellationToken cancellationToken = default)
        {
            if (!(await Exist(entity, cancellationToken)))
                throw new Exception($"No existe el Pais {entity.Code}-{entity.Name}");
        }

        public async Task<bool> Exist(Country entity, CancellationToken cancellationToken = default)
        {
            return (await _countryRepository.Get(entity.Id, cancellationToken)) != null;
        }

        public async Task Code(Country entity, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(entity.Code))
                throw new Exception("El codigo esta vacío.");

            if (entity.Id > 0)
                return;

            var categoryPersisted = await _countryRepository.Get(entity.Code, cancellationToken);

            if (categoryPersisted != null)
                throw new Exception($"El codigo {entity.Code} ya existe.");
        }

        public async Task Name(Country entity, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(entity.Name))
                throw new Exception("El nombre esta vacío.");
        }
    }
}
