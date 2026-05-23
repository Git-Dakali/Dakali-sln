using Dakali.Interface.Connection;
using Dapper;
using DK.Domain.Sales;
using DK.Repositories.Interface.Base;
using DK.Repositories.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Repositories.Sales
{
    public class HistoricSaleRepository : IRepositoryReferenceEntity<Sale, HistoricSale>
    {
        private ISession _session;
        private StoredFileRepository _storedFileRepository;

        public HistoricSaleRepository(ISession session, StoredFileRepository storedFileRepository)
        { 
            _session = session;
            _storedFileRepository = storedFileRepository;
        }

        public async Task<HistoricSale> Create(Sale parent, HistoricSale entity, CancellationToken cancellation = default)
        {
            var query = @"
            INSERT INTO dbo.HistoricSale (SaleId, CreationDate, State, Description, StoredFileId)
            OUTPUT INSERTED.*
            VALUES (@SaleId, @CreationDate, @State, @Description, @StoredFileId);";
            entity.State = SaleState.Creado;
            var rowDapper = await _session.Connection.QuerySingleAsync(new CommandDefinition(query,
                new
                {
                    SaleId = parent.Id,
                    CreationDate = DateTime.Now,
                    parent.State,
                    Description = entity.Description ?? string.Empty,
                    StoredFileId = entity.StoredFile?.Id,
                }, transaction: _session.Transaction, cancellationToken: cancellation));

            if (rowDapper is null)
                throw new Exception("No se pudo crear el historico de venta");

            var historicSale = await Map(rowDapper);

            return historicSale;
        }

        public async Task Delete(Sale parent, HistoricSale entity, CancellationToken cancellation = default)
        {
            throw new NotImplementedException();
        }

        public async Task Delete(Sale parent, IEnumerable<HistoricSale> entities, CancellationToken cancellation = default)
        {
            throw new NotImplementedException();
        }

        public async Task<HistoricSale> Get(Sale parent, long id, CancellationToken cancellation = default)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<HistoricSale>> Get(Sale parent, CancellationToken cancellation = default)
        {
            var query = @"
            SELECT * FROM dbo.HistoricSale WHERE SaleId = @SaleId";
            var rowsDapper = await _session.Connection.QueryAsync(new CommandDefinition(query, new { SaleId = parent.Id, }, transaction: _session.Transaction, cancellationToken: cancellation));

            if (rowsDapper is null)
                return Enumerable.Empty<HistoricSale>();

            var historicSales = new List<HistoricSale>();

            foreach (var rowDapper in rowsDapper)
                historicSales.Add(await Map(rowDapper));

            return historicSales;
        }

        public async Task<IEnumerable<HistoricSale>> SyncCollection(Sale parent, IEnumerable<HistoricSale> entities, CancellationToken cancellation = default)
        {
            throw new NotImplementedException();
        }

        public async Task<HistoricSale> Update(Sale parent, HistoricSale entity, CancellationToken cancellation = default)
        {
            throw new NotImplementedException();
        }

        public async Task<HistoricSale> Map(dynamic rowDapper, CancellationToken cancellation = default)
        {
            var historic = new HistoricSale();
            historic.Id = rowDapper.Id;
            historic.CreationDate = rowDapper.CreationDate;
            historic.Description = rowDapper.Description;
            historic.State = Enum.Parse<SaleState>(rowDapper.State);
            if (rowDapper.StoredFileId != null)
                historic.StoredFile = await _storedFileRepository.Get((long)rowDapper.StoredFileId, cancellation);

            return historic;
        }
    }
}
