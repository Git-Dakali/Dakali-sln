using DK.Domain.RoadMaps;
using DK.Repositories.RoadMaps;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Validator.RoadMaps
{
    public class RoadMapSaleValidator
    {
        RoadMapRepository _roadMapRepository;

        public RoadMapSaleValidator(RoadMapRepository roadMapRepository) 
        {
            _roadMapRepository = roadMapRepository;
        }

        public async Task AssignRoadMap(RoadMapSale roadMapSale, RoadMap roadMap, CancellationToken cancellationToken = default)
        {
            var roadMapPersisted = await _roadMapRepository.Get(roadMapSale.Sale, cancellationToken);

            if (roadMapPersisted != null)
                throw new Exception($"La venta {roadMapSale.Sale.Number} ya se encuentra asignado a la hoja de ruta {roadMapPersisted.Number}.");

            if (roadMap.State == RoadMapState.Finalizado)
                throw new Exception($"La venta {roadMapSale.Sale.Number}, no se puede asignar a la hoja de ruta {roadMap.Number} por que se encuentra Finalizado");
        }

        public async Task UnassignRoadMap(RoadMapSale roadMapSale, RoadMap roadMap, CancellationToken cancellationToken = default)
        {
            var roadMapPersisted = await _roadMapRepository.Get(roadMapSale.Sale, cancellationToken);

            if (roadMapPersisted is null)
                throw new Exception($"La venta {roadMapSale.Sale.Number} no se encuentra asignado una hoja de ruta.");

            if (roadMapPersisted.Id != roadMap.Id)
                throw new Exception($"La venta {roadMapSale.Sale.Number} se encuentra asignado una hoja de ruta {roadMapPersisted.Number}.");

            if (roadMapPersisted.State == RoadMapState.EnViaje)
                throw new Exception($"La venta {roadMapSale.Sale.Number}, no se puede desasignar de la hoja de ruta {roadMap.Number} por que se encuentra En Viaje");

            if (roadMapPersisted.State == RoadMapState.Finalizado)
                throw new Exception($"La venta {roadMapSale.Sale.Number}, no se puede desasignar de la hoja de ruta {roadMap.Number} por que se encuentra Finalizado");
        }
    }
}
