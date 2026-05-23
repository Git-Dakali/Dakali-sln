using Dakali.Domine;
using DK.Domain.Sales;
using DK.Repositories.Sales;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Process.Sales
{
    public class HistoricSaleProcess
    {
        private HistoricSaleRepository _historicSaleRepository;
        private SaleRepository _saleRepository;


        public HistoricSaleProcess(HistoricSaleRepository historicSaleRepository, SaleRepository saleRepository)
        {
            _historicSaleRepository = historicSaleRepository;
            _saleRepository = saleRepository;
        }

        public async Task<HistoricSale> Create(Sale parent, string description, StoredFile storedFile = null, CancellationToken cancellation = default)
        {
            var salePersisted = await _saleRepository.Get(parent.Id, true, cancellation);
            return await _historicSaleRepository.Create(salePersisted, new HistoricSale{ State = salePersisted.State, CreationDate = DateTime.Now, Description = description, StoredFile = storedFile }, cancellation);
        }

        public async Task<IEnumerable<HistoricSale>> Get(Sale parent, CancellationToken cancellation = default)
        {
            return await _historicSaleRepository.Get(parent, cancellation);
        }
    }
}
