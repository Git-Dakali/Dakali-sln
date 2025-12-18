using DK.Domain.Products;
using DK.Repositories.Products;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Validator
{
    public class StockStateValidator
    {
        private StockStateRepository _stockStateRepository;

        public StockStateValidator(StockStateRepository stockStateRepository)
        {
            _stockStateRepository = stockStateRepository ?? throw new ArgumentNullException("StockStateRepository");
        }

        public async Task Create(StockState state, CancellationToken cancellationToken = default)
        {
            await Code(state, cancellationToken);
            await Name(state, cancellationToken);
        }

        public async Task Update(StockState state, CancellationToken cancellationToken = default)
        {
            if (!(await Exist(state, cancellationToken)))
                throw new Exception($"No existe el estado {state.Code}-{state.Name}");

            await Code(state, cancellationToken);
            await Name(state, cancellationToken);
        }

        public async Task Delete(StockState state, CancellationToken cancellationToken = default)
        {
            if (!(await Exist(state, cancellationToken)))
                throw new Exception($"No existe el estado {state.Code}-{state.Name}");
        }

        public async Task<bool> Exist(StockState state, CancellationToken cancellationToken = default)
        {
            return (await _stockStateRepository.Get(state.Id, cancellationToken)) != null;
        }

        public async Task Code(StockState state, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(state.Code))
                throw new Exception("El codigo esta vacío.");

            if (state.Id > 0)
                return;

            var statePersisted = await _stockStateRepository.Get(state.Code, cancellationToken);

            if (statePersisted != null)
                throw new Exception($"El codigo {state.Code} ya existe.");
        }

        public async Task Name(StockState state, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(state.Name))
                throw new Exception("El nombre esta vacío.");
        }


    }
}
