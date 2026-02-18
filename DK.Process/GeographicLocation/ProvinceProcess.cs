using DK.Domain.GeographicLocation;
using DK.Repositories.GeographicLocation;
using DK.Validator.GeographicLocation;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Process.GeographicLocation
{
    public class ProvinceProcess
    {
        private ProvinceRepository _provinceRepository;
        private ProvinceValidator _provinceValidator;

        public ProvinceProcess(ProvinceRepository provinceRepository, ProvinceValidator provinceValidator)
        {
            _provinceRepository = provinceRepository;
            _provinceValidator = provinceValidator;
        }

        public async Task<IEnumerable<Province>> GetAll(CancellationToken cancellationToken = default)
        {
            return await _provinceRepository.GetAll(cancellationToken);
        }

        public async Task<Province> Get(long id, CancellationToken cancellationToken = default)
        {
            return await _provinceRepository.Get(id, cancellationToken);
        }

        public async Task<Province> Get(string code, CancellationToken cancellationToken = default)
        {
            return await _provinceRepository.Get(code, cancellationToken);
        }

        public async Task<IEnumerable<Province>> Get(Country country, CancellationToken cancellationToken = default)
        {
            return await _provinceRepository.Get(country, cancellationToken);
        }

        public async Task<Province> Create(Province entity, CancellationToken cancellationToken = default)
        {

            await _provinceValidator.Create(entity, cancellationToken);

            return await _provinceRepository.Create(entity, cancellationToken);
        }

        public async Task<Province> Update(Province entity, CancellationToken cancellationToken = default)
        {
            await _provinceValidator.Update(entity, cancellationToken);

            return await _provinceRepository.Update(entity, cancellationToken);
        }

        public async Task Delete(Province entity, CancellationToken cancellationToken = default)
        {
            await _provinceValidator.Delete(entity, cancellationToken);
            await _provinceRepository.Delete(entity, cancellationToken);
        }
    }
}
