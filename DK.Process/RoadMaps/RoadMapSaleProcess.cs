using DK.Domain.RoadMaps;
using DK.Domain.Sales;
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

        public async Task<RoadMapSale> Get(RoadMap roadMap, Sale sale, CancellationToken cancellationToken = default)
        {
            return await _roadMapSaleRepository.Get(roadMap, sale, cancellationToken);
        }

        public async Task AssignRoadMap(RoadMap roadMap, RoadMapSale roadMapSale, CancellationToken cancellationToken = default)
        {
            await _roadMapSaleValidator.AssignRoadMap(roadMap, roadMapSale, cancellationToken);
            await _roadMapSaleRepository.Create(roadMap, roadMapSale, cancellationToken);
        }

        public async Task UnassignRoadMap(RoadMap roadMap, RoadMapSale roadMapSale, CancellationToken cancellationToken = default)
        {
            await _roadMapSaleValidator.UnassignRoadMap(roadMap, roadMapSale, cancellationToken);
            await _roadMapSaleRepository.Delete(roadMap, roadMapSale, cancellationToken);
        }
    }
}
