using Dakali.Domine;
using DK.Domain.RoadMaps;
using DK.Domain.Sales;
using DK.Process.Sales;
using DK.Repositories.RoadMaps;
using DK.Validator.RoadMaps;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Process.RoadMaps
{
    public class RoadMapProcess
    {
        private RoadMapRepository _roadMapRepository;
        private RoadMapValidator _roadMapValidator;
        private SaleProcess _saleProcess;

        public RoadMapProcess(RoadMapRepository roadMapRepository, RoadMapValidator roadMapValidator, SaleProcess saleProcess)
        {
            _roadMapRepository = roadMapRepository;
            _roadMapValidator = roadMapValidator;
            _saleProcess = saleProcess;
            
        }

        public async Task<IEnumerable<RoadMap>> GetAll(CancellationToken cancellationToken = default)
        {
            return await _roadMapRepository.GetAll(cancellationToken);
        }

        public async Task<ResultPage<RoadMap>> GetPage(RoadMapFilter roadMapFilter, CancellationToken cancellationToken = default)
        {
            return await _roadMapRepository.GetPage(roadMapFilter, cancellationToken);
        }

        public async Task<RoadMap> Get(long id, CancellationToken cancellationToken = default)
        {
            return await _roadMapRepository.Get(id, cancellationToken);
        }

        public async Task<RoadMap> Get(Sale sale, CancellationToken cancellationToken = default)
        {
            return await _roadMapRepository.Get(sale, cancellationToken);
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

        public async Task OnTrip(RoadMap roadMap, CancellationToken cancellationToken = default)
        {
            await _roadMapValidator.OnTrip(roadMap, cancellationToken);
            await _roadMapRepository.OnTrip(roadMap, cancellationToken);

            foreach (var sale in roadMap.Sales.Select(x => x.Sale))
                await _saleProcess.OnTrip(sale, cancellationToken);
        }

        public async Task FinishTrip(RoadMap roadMap, CancellationToken cancellationToken = default)
        {
            await _roadMapValidator.FinishTrip(roadMap, cancellationToken);
            await _roadMapRepository.FinishTrip(roadMap, cancellationToken);
        }
    }
}
