using DK.Domain.Locations;
using DK.Domain.RoadMaps;
using DK.Domain.Sales;
using DK.Process.Locations;
using DK.Process.Product;
using DK.Process.Sales;
using DK.Repositories.RoadMaps;
using DK.Repositories.Sales;
using DK.Validator.RoadMaps;
using DK.Validator.Sales;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Process.RoadMaps
{
    public class RoadMapProcess
    {
        private RoadMapRepository _roadMapRepository;
        private RoadMapValidator _roadMapValidator;

        public RoadMapProcess(RoadMapRepository roadMapRepository, RoadMapValidator roadMapValidator)
        {
            _roadMapRepository = roadMapRepository;
            _roadMapValidator = roadMapValidator;
            
        }

        public async Task<IEnumerable<RoadMap>> GetAll(CancellationToken cancellationToken = default)
        {
            return await _roadMapRepository.GetAll(cancellationToken);
        }

        public async Task<RoadMap> Get(long id, CancellationToken cancellationToken = default)
        {
            return await _roadMapRepository.Get(id, cancellationToken);
        }

        public async Task<RoadMap> Create(RoadMap roadMap, CancellationToken cancellationToken = default)
        {
            await _roadMapValidator.Create(roadMap, cancellationToken);

            var saleResult = await _roadMapRepository.Create(roadMap, cancellationToken);
            
            return await _roadMapRepository.Get(saleResult.Id, cancellationToken);
        }

        public async Task<RoadMap> Update(RoadMap roadMap, CancellationToken cancellationToken = default)
        {
            await _roadMapValidator.Update(roadMap, cancellationToken);
            return await _roadMapRepository.Update(roadMap, cancellationToken);
        }

        public async Task Delete(RoadMap roadMap, CancellationToken cancellationToken = default)
        {
            await _roadMapValidator.Delete(roadMap, cancellationToken);
            await _roadMapRepository.Delete(roadMap, cancellationToken);
        }
    }
}
