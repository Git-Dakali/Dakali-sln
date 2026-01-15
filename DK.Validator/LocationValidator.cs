using DK.Domain.Locations;
using DK.Repositories.Locations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Validator
{
    public class LocationValidator
    {
        private LocationRepository _locationRepository;
        private HallwayRepository _hallwayRepository;
        private ColumnRepository _columnRepository;
        private LevelRepository _levelRepository; 
        private LocationStateRepository _locationStateRepository;

        public LocationValidator(LocationRepository locationRepository, HallwayRepository hallwayRepository, ColumnRepository columnRepository, LevelRepository levelRepository, LocationStateRepository locationStateRepository)
        {
            _locationRepository = locationRepository ?? throw new ArgumentNullException("LocationRepository");
            _columnRepository = columnRepository ?? throw new ArgumentNullException("ColumnRepository");
            _hallwayRepository = hallwayRepository ?? throw new ArgumentNullException("HallwayRepository");
            _levelRepository = levelRepository ?? throw new ArgumentNullException("LevelRepository");
            _locationStateRepository = locationStateRepository ?? throw new ArgumentNullException("LocationStateRepository");
        }

        public async Task Create(Location location, CancellationToken cancellationToken = default)
        {
            if (await Exist(location, cancellationToken))
                throw new Exception($"Ya existe la ubicacion {location.Hallway.Name}-{location.Column.Name}-{location.Level.Name}");

            await Hallway(location, cancellationToken);
            await Column(location, cancellationToken);
            await Level(location, cancellationToken);
            await State(location, cancellationToken);
        }

        public async Task Update(Location location, CancellationToken cancellationToken = default)
        {
            if (!(await Exist(location, cancellationToken)))
                throw new Exception($"No existe la ubicacion {location.Hallway.Name}-{location.Column.Name}-{location.Level.Name}");

            await Hallway(location, cancellationToken);
            await Column(location, cancellationToken);
            await Level(location, cancellationToken);
            await State(location, cancellationToken);
        }

        public async Task Delete(Location location, CancellationToken cancellationToken = default)
        {
            if (!(await Exist(location, cancellationToken)))
                throw new Exception($"No existe la ubicacion {location.Hallway.Name}-{location.Column.Name}-{location.Level.Name}");
        }

        public async Task<bool> Exist(Location location, CancellationToken cancellationToken = default)
        {
            return (await _locationRepository.Get(location.Hallway, location.Column, location.Level, cancellationToken)) != null;
        }

        public async Task Hallway(Location location, CancellationToken cancellationToken = default)
        {
            if (location.Hallway is null)
                throw new Exception("El pasillo esta vacio.");

            var persisted = await _hallwayRepository.Get(location.Hallway.Id, cancellationToken);

            if (persisted is null)
                throw new Exception($"El pasillo {location.Hallway.Name} no existe.");
        }

        public async Task Column(Location location, CancellationToken cancellationToken = default)
        {
            if (location.Column is null)
                throw new Exception("La columna esta vacio.");

            var persisted = await _columnRepository.Get(location.Column.Id, cancellationToken);

            if (persisted is null)
                throw new Exception($"La columna {location.Column.Name} no existe.");
        }

        public async Task Level(Location location, CancellationToken cancellationToken = default)
        {
            if (location.Level is null)
                throw new Exception("El nivel esta vacio.");

            var persisted = await _levelRepository.Get(location.Level.Id, cancellationToken);

            if (persisted is null)
                throw new Exception($"El nivel {location.Level.Name} no existe.");
        }

        public async Task State(Location location, CancellationToken cancellationToken = default)
        {
            if (location.State is null)
                throw new Exception("El estado esta vacio.");

            var persisted = await _locationStateRepository.Get(location.State.Id, cancellationToken);

            if (persisted is null)
                throw new Exception($"El estado {location.State.Name} no existe.");
        }
    }
}
