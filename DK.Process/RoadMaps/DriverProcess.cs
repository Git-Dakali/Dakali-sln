using DK.Domain.RoadMaps;
using DK.Repositories.RoadMaps;
using DK.Validator.RoadMaps;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Process.RoadMaps
{
    public class DriverProcess
    {
        private DriverRepository _driverRepository;
        private DriverValidator _driverValidator;

        public DriverProcess(DriverRepository driverRepository, DriverValidator driverValidator)
        {
            _driverRepository = driverRepository;
            _driverValidator = driverValidator;
        }

        public async Task<IEnumerable<Driver>> GetAll(CancellationToken cancellationToken = default)
        {
            return await _driverRepository.GetAll(cancellationToken);
        }

        public async Task<Driver> Get(long id, CancellationToken cancellationToken = default)
        {
            return await _driverRepository.Get(id, cancellationToken);
        }

        public async Task<Driver> Create(Driver driver, CancellationToken cancellationToken = default)
        {
            await _driverValidator.Create(driver, cancellationToken);
            return await _driverRepository.Create(driver, cancellationToken);
        }

        public async Task<Driver> Update(Driver driver, CancellationToken cancellationToken = default)
        {
            await _driverValidator.Update(driver, cancellationToken);
            return await _driverRepository.Update(driver, cancellationToken);
        }

        public async Task Delete(Driver driver, CancellationToken cancellationToken = default)
        {
            await _driverValidator.Delete(driver, cancellationToken);
            await _driverRepository.Delete(driver, cancellationToken);
        }
    }
}
