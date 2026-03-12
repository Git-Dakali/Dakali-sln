using DK.Domain.RoadMaps;
using DK.Repositories.RoadMaps;
using DK.Validator.RoadMaps;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Process.RoadMaps
{
    public class RoadMapSaleProcess
    {
        private RoadMapSaleValidator _roadMapSaleValidator;
        private RoadMapSaleRepository _roadMapSaleRepository;

        public RoadMapSaleProcess(RoadMapSaleValidator roadMapSaleValidator, RoadMapSaleRepository roadMapSaleRepository) 
        {
            _roadMapSaleValidator = roadMapSaleValidator;
            _roadMapSaleRepository = roadMapSaleRepository;
        }

        public async Task<IEnumerable<RoadMapSale>> Get(RoadMap roadMap, CancellationToken cancellationToken = default)
        {
            return await _roadMapSaleRepository.Get(roadMap, cancellationToken);
        }

        public async Task AssignRoadMap(RoadMapSale roadMapSale, RoadMap roadMap, CancellationToken cancellationToken = default)
        {
            await _roadMapSaleValidator.AssignRoadMap(roadMapSale, roadMap, cancellationToken);
            await _roadMapSaleRepository.Create(roadMap, roadMapSale, cancellationToken);
        }

        public async Task UnassignRoadMap(RoadMapSale roadMapSale, RoadMap roadMap, CancellationToken cancellationToken = default)
        {
            await _roadMapSaleValidator.UnassignRoadMap(roadMapSale, roadMap, cancellationToken);
            await _roadMapSaleRepository.Delete(roadMap, roadMapSale, cancellationToken);
        }
    }
}
