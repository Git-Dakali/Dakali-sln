using DK.Domain.Sales;
using DK.Repositories.Sales;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Validator.Sales
{
    public class OriginSaleValidator
    {
        private OriginSaleRepository _originSaleRepository;

        public OriginSaleValidator(OriginSaleRepository originSaleRepository)
        {
            _originSaleRepository = originSaleRepository ?? throw new ArgumentNullException("OriginSaleRepository");
        }

        public async Task Create(OriginSale category, CancellationToken cancellationToken = default)
        {
            await Code(category, cancellationToken);
            await Name(category, cancellationToken);
        }

        public async Task Update(OriginSale entity, CancellationToken cancellationToken = default)
        {
            if (!await Exist(entity, cancellationToken))
                throw new Exception($"No existe el Origen de Venta {entity.Name}");

            await Name(entity, cancellationToken);
        }

        public async Task Delete(OriginSale entity, CancellationToken cancellationToken = default)
        {
            if (!await Exist(entity, cancellationToken))
                throw new Exception($"No existe el Origen de Venta {entity.Name}");
        }

        public async Task<bool> Exist(OriginSale entity, CancellationToken cancellationToken = default)
        {
            return await _originSaleRepository.Get(entity.Id, cancellationToken) != null;
        }

        public async Task Code(OriginSale entity, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(entity.Code))
                throw new Exception("El codigo esta vacío.");

            if (entity.Id > 0)
                return;

            var persisted = await _originSaleRepository.Get(entity.Code, cancellationToken);

            if (persisted != null)
                throw new Exception($"El codigo {entity.Code} ya existe.");
        }

        public async Task Name(OriginSale entity, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(entity.Name))
                throw new Exception("El nombre esta vacío.");
        }
    }
}
