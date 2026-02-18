using DK.Domain.GeographicLocation;
using DK.Repositories.GeographicLocation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Validator.GeographicLocation
{
    public class CityValidator
    {
        private CityRepository _cityRepository;
        private ProvinceRepository _provinceRepository;

        public CityValidator(CityRepository cityRepository, ProvinceRepository provinceRepository)
        {
            _provinceRepository = provinceRepository ?? throw new ArgumentNullException("ProvinceRepository");
            _cityRepository = cityRepository ?? throw new ArgumentNullException("CityRepository");
        }

        public async Task Create(City entity, CancellationToken cancellationToken = default)
        {
            await ZipCode(entity, cancellationToken);
            await Name(entity, cancellationToken);
            await Province(entity, cancellationToken);
        }

        public async Task Update(City entity, CancellationToken cancellationToken = default)
        {
            if (!(await Exist(entity, cancellationToken)))
                throw new Exception($"No existe la Localidad {entity.ZipCode}-{entity.Name}");

            await ZipCode(entity, cancellationToken);
            await Name(entity, cancellationToken);
            await Province(entity, cancellationToken);
        }

        public async Task Delete(City entity, CancellationToken cancellationToken = default)
        {
            if (!(await Exist(entity, cancellationToken)))
                throw new Exception($"No existe la Localidad {entity.ZipCode}-{entity.Name}");
        }

        public async Task<bool> Exist(City entity, CancellationToken cancellationToken = default)
        {
            return (await _cityRepository.Get(entity.Id, cancellationToken)) != null;
        }

        public async Task ZipCode(City entity, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(entity.ZipCode))
                throw new Exception("El codigo postal esta vacío.");
        }

        public async Task Name(City entity, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(entity.Name))
                throw new Exception("El nombre esta vacío.");
        }

        public async Task Province(City entity, CancellationToken cancellationToken = default)
        {
            if (entity.Province is null)
                throw new Exception("La Provincia esta vacío.");

            var province = await _provinceRepository.Get(entity.Province.Id, cancellationToken);

            if (province is null)
                throw new Exception($"La Provincia {entity.Province.Code}-{entity.Province.Name} no existe.");
        }
    }
}
