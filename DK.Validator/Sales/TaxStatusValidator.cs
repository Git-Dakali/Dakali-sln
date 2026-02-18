using DK.Domain.Sales;
using DK.Repositories.Sales;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Validator.Sales
{
    public class TaxStatusValidator
    {
        private TaxStatusRepository _taxStatusRepository;

        public TaxStatusValidator(TaxStatusRepository taxStatusRepository)
        {
            _taxStatusRepository = taxStatusRepository ?? throw new ArgumentNullException("TaxStatusRepository");
        }

        public async Task Create(TaxStatus taxStatus, CancellationToken cancellationToken = default)
        {
            await Code(taxStatus, cancellationToken);
            await Name(taxStatus, cancellationToken);
        }

        public async Task Update(TaxStatus taxStatus, CancellationToken cancellationToken = default)
        {
            if (!(await Exist(taxStatus, cancellationToken)))
                throw new Exception($"No existe la condicion fiscal {taxStatus.Code}-{taxStatus.Name}");

            await Code(taxStatus, cancellationToken);
            await Name(taxStatus, cancellationToken);
        }

        public async Task Delete(TaxStatus taxStatus, CancellationToken cancellationToken = default)
        {
            if (!(await Exist(taxStatus, cancellationToken)))
                throw new Exception($"No existe la condicion fiscal {taxStatus.Code}-{taxStatus.Name}");
        }

        public async Task<bool> Exist(TaxStatus taxStatus, CancellationToken cancellationToken = default)
        {
            return (await _taxStatusRepository.Get(taxStatus.Id, cancellationToken)) != null;
        }

        public async Task Code(TaxStatus taxStatus, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(taxStatus.Code))
                throw new Exception("El codigo esta vacío.");

            if (taxStatus.Id > 0)
                return;

            var persisted = await _taxStatusRepository.Get(taxStatus.Code, cancellationToken);

            if (persisted != null)
                throw new Exception($"El codigo {taxStatus.Code} ya existe.");
        }

        public async Task Name(TaxStatus taxStatus, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(taxStatus.Name))
                throw new Exception("El nombre esta vacío.");
        }
    }
}
