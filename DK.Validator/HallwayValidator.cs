using DK.Domain.Locations;
using DK.Repositories.Locations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Validator
{
    public class HallwayValidator
    {
        private HallwayRepository _hallwayRepository;

        public HallwayValidator(HallwayRepository hallwayRepository)
        {
            _hallwayRepository = hallwayRepository ?? throw new ArgumentNullException("HallwayRepository");
        }

        public async Task Create(Hallway hallway, CancellationToken cancellationToken = default)
        {
            await Code(hallway, cancellationToken);
            await Name(hallway, cancellationToken);
        }

        public async Task Update(Hallway hallway, CancellationToken cancellationToken = default)
        {
            if (!(await Exist(hallway, cancellationToken)))
                throw new Exception($"No existe el pasillo {hallway.Code}-{hallway.Name}");

            await Code(hallway, cancellationToken);
            await Name(hallway, cancellationToken);
        }

        public async Task Delete(Hallway hallway, CancellationToken cancellationToken = default)
        {
            if (!(await Exist(hallway, cancellationToken)))
                throw new Exception($"No existe el pasillo {hallway.Code}-{hallway.Name}");
        }

        public async Task<bool> Exist(Hallway hallway, CancellationToken cancellationToken = default)
        {
            return (await _hallwayRepository.Get(hallway.Id, cancellationToken)) != null;
        }

        public async Task Code(Hallway hallway, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(hallway.Code))
                throw new Exception("El codigo esta vacío.");

            if (hallway.Id > 0)
                return;

            var persisted = await _hallwayRepository.Get(hallway.Code, cancellationToken);

            if (persisted != null)
                throw new Exception($"El codigo {hallway.Code} ya existe.");
        }

        public async Task Name(Hallway hallway, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(hallway.Name))
                throw new Exception("El nombre esta vacío.");
        }

    }
}
