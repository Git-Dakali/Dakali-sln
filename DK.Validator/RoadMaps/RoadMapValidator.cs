using DK.Domain.RoadMaps;
using DK.Repositories.RoadMaps;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Validator.RoadMaps
{
    public class RoadMapValidator
    {
        public RoadMapRepository _roadMapRepository;
        public RoadMapSaleValidator _roadMapsaleValidator;

        public RoadMapValidator(RoadMapRepository roadMapRepository, RoadMapSaleValidator roadMapsaleValidator)
        {
            _roadMapRepository = roadMapRepository ?? throw new ArgumentNullException("RoadMapRepository");
            _roadMapsaleValidator = roadMapsaleValidator ?? throw new ArgumentNullException("SaleValidator");
        }

        public async Task Create(RoadMap roadMap, CancellationToken cancellationToken = default)
        {
            await Number(roadMap, cancellationToken);
            await Driver(roadMap, cancellationToken);
            await State(roadMap, cancellationToken);
            await Sales(roadMap, cancellationToken);

            if (roadMap.State != RoadMapState.Creado)
                throw new Exception($"El estado de la hoja de ruta debe ser {RoadMapState.Creado}");

            foreach (var sale in roadMap.Sales)
                await _roadMapsaleValidator.AssignRoadMap(sale, roadMap, cancellationToken);
        }

        public async Task Update(RoadMap roadMap, CancellationToken cancellationToken = default)
        {
            await Number(roadMap, cancellationToken);
            await Driver(roadMap, cancellationToken);
            await State(roadMap, cancellationToken);
            await Sales(roadMap, cancellationToken);
        }

        public async Task Delete(RoadMap roadMap, CancellationToken cancellationToken = default)
        {
            if (!(await Exist(roadMap, cancellationToken)))
                throw new Exception($"No existe la hoja de ruta {roadMap.Number}");

            if (roadMap.State == RoadMapState.EnViaje)
                throw new Exception($"No se puede eliminar la Hoja de Ruta {roadMap.Number} en estado En Viaje.");

            if (roadMap.State == RoadMapState.Finalizado)
                throw new Exception($"No se puede eliminar la Hoja de Ruta {roadMap.Number} en estado Finalizado.");
        }

        public async Task<bool> Exist(RoadMap sale, CancellationToken cancellationToken = default)
        {
            return (await _roadMapRepository.Get(sale.Id, cancellationToken)) != null;
        }

        
        public async Task Number(RoadMap sale, CancellationToken cancellationToken = default) { }

        public async Task Driver(RoadMap sale, CancellationToken cancellationToken = default)
        {
            if (sale.Driver is null)
                throw new Exception("Debe ingresar un chofer.");
        }

        public async Task State(RoadMap sale, CancellationToken cancellationToken = default) { 
            
        }

        public async Task Sales(RoadMap roadMap, CancellationToken cancellationToken = default)
        {
            if (roadMap.Sales != null)
            {
                var sales = roadMap.Sales.GroupBy(s => s.Sale.Id).ToList();
                if (sales.Any(s => s.Count() > 1))
                    throw new Exception("Existe ventas asignado mas de una vez.");
            }

            if (roadMap.State == RoadMapState.Creado)
                return;

            if (roadMap.Sales is null)
                throw new Exception("No posee ventas asignadas");

            if (roadMap.Sales.Count() <= 0)
                throw new Exception("No posee ventas asignadas");
        }
    }
}
