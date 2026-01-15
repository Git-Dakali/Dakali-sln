using DK.Domain.Locations;
using DK.Repositories.Locations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Validator
{
    public class LocationStateValidator
    {
        private LocationStateRepository _locationStateRepository;

        public LocationStateValidator(LocationStateRepository stockStateRepository)
        {
            _locationStateRepository = stockStateRepository ?? throw new ArgumentNullException("LocationStateRepository");
        }

        public async Task Create(LocationState state, CancellationToken cancellationToken = default)
        {
            await Code(state, cancellationToken);
            await Name(state, cancellationToken);
        }

        public async Task Update(LocationState state, CancellationToken cancellationToken = default)
        {
            if (!(await Exist(state, cancellationToken)))
                throw new Exception($"No existe el estado {state.Code}-{state.Name}");

            await Code(state, cancellationToken);
            await Name(state, cancellationToken);
        }

        public async Task Delete(LocationState state, CancellationToken cancellationToken = default)
        {
            if (!(await Exist(state, cancellationToken)))
                throw new Exception($"No existe el estado {state.Code}-{state.Name}");
        }

        public async Task<bool> Exist(LocationState state, CancellationToken cancellationToken = default)
        {
            return (await _locationStateRepository.Get(state.Id, cancellationToken)) != null;
        }

        public async Task Code(LocationState state, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(state.Code))
                throw new Exception("El codigo esta vacío.");

            if (state.Id > 0)
                return;

            var statePersisted = await _locationStateRepository.Get(state.Code, cancellationToken);

            if (statePersisted != null)
                throw new Exception($"El codigo {state.Code} ya existe.");
        }

        public async Task Name(LocationState state, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(state.Name))
                throw new Exception("El nombre esta vacío.");
        }


    }
}
