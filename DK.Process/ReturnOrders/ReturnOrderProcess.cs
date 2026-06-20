using Dakali.Domine;
using DK.Domain.ReturnOrders;
using DK.Repositories.ReturnOrders;
using DK.Validator.ReturnOrders;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Process.ReturnOrders
{
    public class ReturnOrderProcess
    {
        private ReturnOrderRepository _returnOrderRepository;
        private ReturnOrderValidator _returnOrderValidator;
        
        public ReturnOrderProcess(ReturnOrderRepository returnOrderRepository, ReturnOrderValidator returnOrderValidator)
        {
            _returnOrderRepository = returnOrderRepository;
            _returnOrderValidator = returnOrderValidator;
        }

        public async Task<ReturnOrder> Create(ReturnOrder entity, CancellationToken cancellationToken = default)
        {
            await _returnOrderValidator.Create(entity, cancellationToken);
            return await _returnOrderRepository.Create(entity, cancellationToken);
        }

        public async Task<IEnumerable<ReturnOrder>> GetAll(CancellationToken cancellationToken = default)
        {
            return await _returnOrderRepository.GetAll(cancellationToken);
        }

        public async Task<ReturnOrder> Get(long id, CancellationToken cancellation = default)
        {
            return await _returnOrderRepository.Get(id, cancellation);
        }

        public async Task<ResultPage<ReturnOrder>> GetPage(ReturnOrderFilter returnOrderFilter, CancellationToken cancellationToken = default)
        {
            return await _returnOrderRepository.GetPage(returnOrderFilter, cancellationToken);
        }

        public async Task<ReturnOrder> Returned(ReturnOrder returnOrder, CancellationToken cancellationToken)
        {
            var salePersisted = await _returnOrderRepository.Get(returnOrder.Id, cancellationToken);

            await _returnOrderValidator.Returned(salePersisted, cancellationToken);
            await _returnOrderRepository.UpdateState(salePersisted.Id, ReturnOrderState.Devuelto, cancellationToken);

            return await _returnOrderRepository.Get(salePersisted.Id, cancellationToken);
        }

        public async Task<ReturnOrder> Stored(ReturnOrder returnOrder, CancellationToken cancellationToken)
        {
            var salePersisted = await _returnOrderRepository.Get(returnOrder.Id, cancellationToken);

            await _returnOrderValidator.Stored(salePersisted, cancellationToken);
            await _returnOrderRepository.UpdateState(salePersisted.Id, ReturnOrderState.Almacenado, cancellationToken);

            return await _returnOrderRepository.Get(salePersisted.Id, cancellationToken);
        }

        public async Task<ReturnOrder> NotReturned(ReturnOrder returnOrder, CancellationToken cancellationToken)
        {
            var salePersisted = await _returnOrderRepository.Get(returnOrder.Id, cancellationToken);

            await _returnOrderValidator.NotReturned(salePersisted, cancellationToken);
            await _returnOrderRepository.UpdateState(salePersisted.Id, ReturnOrderState.NoDevuelto, cancellationToken);

            return await _returnOrderRepository.Get(salePersisted.Id, cancellationToken);
        }
    }
}
