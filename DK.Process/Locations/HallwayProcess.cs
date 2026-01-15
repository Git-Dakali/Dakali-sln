using DK.Domain.Locations;
using DK.Repositories.Locations;
using DK.Validator;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Process.Locations
{
    public class HallwayProcess
    {
        private HallwayRepository _hallwayRepository;
        private HallwayValidator _hallwayValidator;

        public HallwayProcess(HallwayRepository hallwayRepository, HallwayValidator hallwayValidator)
        {
            _hallwayRepository = hallwayRepository;
            _hallwayValidator = hallwayValidator;
        }

        public async Task<IEnumerable<Hallway>> GetAll(CancellationToken cancellationToken = default)
        {
            return await _hallwayRepository.GetAll(cancellationToken);
        }

        public async Task<Hallway> Get(long id, CancellationToken cancellationToken = default)
        {
            return await _hallwayRepository.Get(id, cancellationToken);
        }

        public async Task<Hallway> Get(string code, CancellationToken cancellationToken = default)
        {
            return await _hallwayRepository.Get(code, cancellationToken);
        }

        public async Task<Hallway> Create(Hallway entity, CancellationToken cancellationToken = default)
        {

            await _hallwayValidator.Create(entity, cancellationToken);

            return await _hallwayRepository.Create(entity, cancellationToken);
        }

        public async Task<Hallway> Update(Hallway entity, CancellationToken cancellationToken = default)
        {
            await _hallwayValidator.Update(entity, cancellationToken);

            return await _hallwayRepository.Update(entity, cancellationToken);
        }

        public async Task Delete(Hallway entity, CancellationToken cancellationToken = default)
        {
            await _hallwayValidator.Delete(entity, cancellationToken);
            await _hallwayRepository.Delete(entity, cancellationToken);
        }
    }
}
