using DK.Domain.GeographicLocation;
using DK.Repositories.GeographicLocation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Validator.GeographicLocation
{
    public class ProvinceValidator
    {
        private CountryRepository _countryRepository;
        private ProvinceRepository _provinceRepository;

        public ProvinceValidator(ProvinceRepository provinceRepository, CountryRepository countryRepository)
        {
            _provinceRepository = provinceRepository ?? throw new ArgumentNullException("ProvinceRepository");
            _countryRepository = countryRepository ?? throw new ArgumentNullException("CountryRepository");
        }

        public async Task Create(Province entity, CancellationToken cancellationToken = default)
        {
            await Code(entity, cancellationToken);
            await Name(entity, cancellationToken);
            await Country(entity, cancellationToken);
        }

        public async Task Update(Province entity, CancellationToken cancellationToken = default)
        {
            if (!(await Exist(entity, cancellationToken)))
                throw new Exception($"No existe el Pais {entity.Code}-{entity.Name}");

            await Code(entity, cancellationToken);
            await Name(entity, cancellationToken);
            await Country(entity, cancellationToken);
        }

        public async Task Delete(Province entity, CancellationToken cancellationToken = default)
        {
            if (!(await Exist(entity, cancellationToken)))
                throw new Exception($"No existe el Pais {entity.Code}-{entity.Name}");
        }

        public async Task<bool> Exist(Province entity, CancellationToken cancellationToken = default)
        {
            return (await _provinceRepository.Get(entity.Id, cancellationToken)) != null;
        }

        public async Task Code(Province entity, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(entity.Code))
                throw new Exception("El codigo esta vacío.");

            if (entity.Id > 0)
                return;

            var categoryPersisted = await _provinceRepository.Get(entity.Code, cancellationToken);

            if (categoryPersisted != null)
                throw new Exception($"El codigo {entity.Code} ya existe.");
        }

        public async Task Name(Province entity, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(entity.Name))
                throw new Exception("El nombre esta vacío.");
        }

        public async Task Country(Province entity, CancellationToken cancellationToken = default)
        {
            if (entity.Country is null)
                throw new Exception("El Pais esta vacío.");

            var country = await _countryRepository.Get(entity.Country.Id, cancellationToken);

            if (country is null)
                throw new Exception($"El Pais {entity.Country.Code}-{entity.Country.Name} no existe.");
        }
    }
}
