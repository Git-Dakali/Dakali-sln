using Dakali.Domine;
using Dakali.Interface.Connection;
using Dapper;
using DK.Domain.ReturnOrders;
using DK.Repositories.Interface.Base;
using DK.Repositories.Sales;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Repositories.ReturnOrders
{
    public class ReturnOrderRepository : IRepository<ReturnOrder>
    {
        private ISession _session;
        private SaleRepository _saleRepository;


        public ReturnOrderRepository(ISession session, SaleRepository saleRepository)
        {
            _session = session;
            _saleRepository = saleRepository;
        }

        public async Task<ReturnOrder> Create(ReturnOrder entity, CancellationToken cancellation = default)
        {
            var query = @"
            INSERT INTO dbo.ReturnOrder(SaleId, ReturnDate, State, SearchString)
            OUTPUT INSERTED.*
            VALUES (@SaleId, @ReturnDate, @State, @SearchString);";

            entity.SearchString = entity.ToString();
            var rowDapper = await _session.Connection.QuerySingleAsync(new CommandDefinition(query, new { SaleId = entity.Sale.Id, entity.ReturnDate, State = ReturnOrderState.PendienteDevolver, entity.SearchString }, transaction: _session.Transaction, cancellationToken: cancellation));
            
            return await Map(rowDapper, cancellation);
        }

        public async Task Delete(ReturnOrder entity, CancellationToken cancellation = default)
        {
            throw new System.NotImplementedException();
        }

        public async Task<ReturnOrder> Get(long id, CancellationToken cancellation = default)
        {
            var query = "select * from dbo.ReturnOrder where IsDeleted = 0 AND Id = @Id";
            var rowDapper = await _session.Connection.QuerySingleOrDefaultAsync(new CommandDefinition(query, new { Id = id }, transaction: _session.Transaction, cancellationToken: cancellation));

            return await Map(rowDapper, cancellation);
        }

        public async Task<IEnumerable<ReturnOrder>> GetAll(CancellationToken cancellation = default)
        {
            var query = "select * from dbo.ReturnOrder where IsDeleted = 0";
            var rowsDapper = await _session.Connection.QueryAsync(new CommandDefinition(query, new { }, transaction: _session.Transaction, cancellationToken: cancellation));

            var roadMaps = new List<ReturnOrder>();
            foreach (var row in rowsDapper)
                roadMaps.Add(await Map(row, cancellation));

            return roadMaps;
        }

        public async Task<ResultPage<ReturnOrder>> GetPage(ReturnOrderFilter saleFilter, CancellationToken cancellationToken = default)
        {
            if (saleFilter is null || saleFilter.CountRows <= 0 || saleFilter.Page <= 0)
                return new ResultPage<ReturnOrder>() { Count = 0, Values = new List<ReturnOrder>() };

            var query = @" select * from dbo.ReturnOrder where IsDeleted = 0";
            var queryCount = @" select COUNT(*) from dbo.ReturnOrder where IsDeleted = 0";

            dynamic filter = new ExpandoObject();

            if (saleFilter.Id != null)
            {
                query += " AND Id = @Id";
                queryCount += " AND Id = @Id";

                filter.Id = saleFilter.Id;
            }

            if (!string.IsNullOrWhiteSpace(saleFilter.SearchString))
            {
                query += " AND CONTAINS(SearchString, @SearchString)";
                queryCount += " AND CONTAINS(SearchString, @SearchString)";

                var values = (saleFilter.SearchString ?? string.Empty).Split(" ").Select(value => value.Trim()).Where(value => !string.IsNullOrWhiteSpace(value)).Select(value => $"\"{value}*\"");

                filter.SearchString = $"({string.Join(" AND ", values)})";
            }

            query += @$"
                ORDER BY Id
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY;

                {queryCount}
            ";

            filter.Offset = (saleFilter.Page - 1) * saleFilter.CountRows;
            filter.PageSize = saleFilter.CountRows;

            var results = await _session.Connection.QueryMultipleAsync(new CommandDefinition(query, filter as object, transaction: _session.Transaction, cancellationToken: cancellationToken));
            var rowsDapper = results.Read().ToList();
            var count = results.Read<long>().Single();

            if (rowsDapper is null)
                return new ResultPage<ReturnOrder>() { Count = 0, Values = new List<ReturnOrder>() };

            var returnOrder = new List<ReturnOrder>();

            foreach (var row in rowsDapper)
                returnOrder.Add(await Map(row));

            return new ResultPage<ReturnOrder>() { Count = count, Values = returnOrder };
        }

        public async Task<ReturnOrder> Update(ReturnOrder entity, CancellationToken cancellation = default)
        {
            throw new System.NotImplementedException();
        }

        public async Task UpdateState(long returnOrderId, ReturnOrderState state, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.ReturnOrder
                SET State = @State,
                    UpdateDate = SYSUTCDATETIME(),
                    Version = Version + 1
                WHERE Id = @Id AND IsDeleted = 0;
            ";

            await _session.Connection.ExecuteAsync(new CommandDefinition(query, new { Id = returnOrderId, State = state.ToString() }, transaction: _session.Transaction, cancellationToken: cancellation));
        }

        public async Task<ReturnOrder> Map(dynamic rowDapper, CancellationToken cancellationToken = default)
        { 
            if(rowDapper is null)
                return null;

            var returnOrder = new ReturnOrder();
            returnOrder.Id = rowDapper.Id;
            returnOrder.SearchString = rowDapper.SearchString;
            returnOrder.CreationDate = rowDapper.CreationDate;
            returnOrder.RemoveDate = rowDapper.RemoveDate;
            returnOrder.UpdateDate = rowDapper.UpdateDate;
            returnOrder.Version = rowDapper.Version;
            returnOrder.Guid = rowDapper.Guid;
            returnOrder.IsDeleted = rowDapper.IsDeleted;
            returnOrder.Number = rowDapper.Number;
            returnOrder.ReturnDate = rowDapper.ReturnDate;
            returnOrder.State = Enum.Parse<ReturnOrderState>(rowDapper.State);
            returnOrder.Sale = await _saleRepository.Get((long)(rowDapper.SaleId ?? 0));

            return returnOrder;
        }
    }
}
