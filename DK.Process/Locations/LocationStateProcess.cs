using DK.Domain.Locations;
using DK.Repositories.Locations;
using DK.Validator;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Process.Locations
{
    public class LocationStateProcess
    {
        private LocationStateRepository _stockStateRepository;
        private LocationStateValidator _stockStateValidator;

        public LocationStateProcess(LocationStateRepository stockStateRepository, LocationStateValidator stockStateValidator)
        {
            _stockStateRepository = stockStateRepository;
            _stockStateValidator = stockStateValidator;
        }

        public async Task<IEnumerable<LocationState>> GetAll(CancellationToken cancellationToken = default)
        {
            return await _stockStateRepository.GetAll(cancellationToken);
        }

        public async Task<LocationState> Get(long id, CancellationToken cancellationToken = default)
        {
            return await _stockStateRepository.Get(id, cancellationToken);
        }

        public async Task<LocationState> Get(string code, CancellationToken cancellationToken = default)
        {
            return await _stockStateRepository.Get(code, cancellationToken);
        }

        public async Task<LocationState> Create(LocationState category, CancellationToken cancellationToken = default)
        {
            
            await _stockStateValidator.Create(category, cancellationToken);

            return await _stockStateRepository.Create(category, cancellationToken);
        }

        public async Task<LocationState> Update(LocationState category, CancellationToken cancellationToken = default)
        {
            await _stockStateValidator.Update(category, cancellationToken);

            return await _stockStateRepository.Update(category, cancellationToken);
        }

        public async Task Delete(LocationState category, CancellationToken cancellationToken = default)
        {
            await _stockStateValidator.Delete(category, cancellationToken);
            await _stockStateRepository.Delete(category, cancellationToken);
        }
    }
}
