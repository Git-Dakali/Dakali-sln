using DK.Domain.Locations;
using DK.Repositories.Locations;
using DK.Validator;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Process.Locations
{
    public class LevelProcess
    {
        private LevelRepository _levelRepository;
        private LevelValidator _levelValidator;

        public LevelProcess(LevelRepository levelRepository, LevelValidator levelValidator)
        {
            _levelRepository = levelRepository;
            _levelValidator = levelValidator;
        }

        public async Task<IEnumerable<Level>> GetAll(CancellationToken cancellationToken = default)
        {
            return await _levelRepository.GetAll(cancellationToken);
        }

        public async Task<Level> Get(long id, CancellationToken cancellationToken = default)
        {
            return await _levelRepository.Get(id, cancellationToken);
        }

        public async Task<Level> Get(string code, CancellationToken cancellationToken = default)
        {
            return await _levelRepository.Get(code, cancellationToken);
        }

        public async Task<Level> Create(Level entity, CancellationToken cancellationToken = default)
        {

            await _levelValidator.Create(entity, cancellationToken);

            return await _levelRepository.Create(entity, cancellationToken);
        }

        public async Task<Level> Update(Level entity, CancellationToken cancellationToken = default)
        {
            await _levelValidator.Update(entity, cancellationToken);

            return await _levelRepository.Update(entity, cancellationToken);
        }

        public async Task Delete(Level entity, CancellationToken cancellationToken = default)
        {
            await _levelValidator.Delete(entity, cancellationToken);
            await _levelRepository.Delete(entity, cancellationToken);
        }
    }
}
