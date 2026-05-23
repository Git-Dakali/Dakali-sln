using Dakali.Domine;
using Dakali.Interface.Connection;
using Dapper;
using DK.Domain.RoadMaps;
using DK.Domain.Sales;
using DK.Repositories.Interface.Base;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Repositories.RoadMaps
{
    public class RoadMapRepository : IRepository<RoadMap>
    {
        private ISession _session;
        private RoadMapSaleRepository _roadMapSaleRepository;
        private DriverRepository _driverRepository;
        
        public RoadMapRepository(ISession session, RoadMapSaleRepository roadMapSaleRepository, DriverRepository driverRepository)
        {
            _session = session;
            _roadMapSaleRepository = roadMapSaleRepository;
            _driverRepository = driverRepository;
        }

        public async Task<RoadMap> Create(RoadMap entity, CancellationToken cancellation = default)
        {
            var query = @"
            INSERT INTO dbo.RoadMap(Date, DriverId, State, SearchString)
            OUTPUT INSERTED.*
            VALUES (@Date, @DriverId, @State, @SearchString);";

            entity.SearchString = entity.ToString();
            var rowDapper = await _session.Connection.QuerySingleAsync(new CommandDefinition(query, new { entity.Date, DriverId = entity.Driver?.Id, State = RoadMapState.Creado.ToString(), entity.SearchString }, transaction: _session.Transaction, cancellationToken: cancellation));
            var roadMap = (RoadMap) await Map(rowDapper, cancellation);
            
            roadMap.Sales = await _roadMapSaleRepository.SyncCollection(roadMap, entity.Sales, cancellation);
            
            return roadMap;
        }

        public async Task Delete(RoadMap entity, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.RoadMap
                   SET IsDeleted = 1,
                       RemoveDate = SYSUTCDATETIME(),
                       UpdateDate = SYSUTCDATETIME(),
                       Version = Version + 1
                 WHERE Id = @id AND IsDeleted = 0;";

            await _session.Connection.ExecuteAsync(new CommandDefinition(query, entity, transaction: _session.Transaction, cancellationToken: cancellation));
        }

        public async Task<RoadMap> Get(long id, CancellationToken cancellation = default)
        {
            var query = "select * from dbo.RoadMap where IsDeleted = 0 AND Id = @Id";
            var rowDapper = await _session.Connection.QuerySingleOrDefaultAsync(new CommandDefinition(query, new { Id = id }, transaction: _session.Transaction, cancellationToken: cancellation));
            
            return await Map(rowDapper, cancellation);
        }

        public async Task<RoadMap> Get(Sale sale, CancellationToken cancellation = default)
        {
            var query = "select * from dbo.RoadMap r where r.IsDeleted = 0 AND r.Id in (SELECT rs.RoadMapId FROM dbo.RoadMapSale rs WHERE rs.SaleId = @Id)";
            
            var rowDapper = await _session.Connection.QuerySingleOrDefaultAsync(new CommandDefinition(query, new { sale.Id }, transaction: _session.Transaction, cancellationToken: cancellation));

            return await Map(rowDapper, cancellation);
        }

        public async Task<IEnumerable<RoadMap>> GetAll(CancellationToken cancellation = default)
        {
            var query = "select * from dbo.RoadMap where IsDeleted = 0";
            var rowsDapper = await _session.Connection.QueryAsync(new CommandDefinition(query, new { }, transaction: _session.Transaction, cancellationToken: cancellation));

            var roadMaps = new List<RoadMap>();
            foreach (var row in rowsDapper)
                roadMaps.Add(await Map(row, cancellation));

            return roadMaps;
        }

        public async Task<ResultPage<RoadMap>> GetPage(RoadMapFilter roadMapFilter, CancellationToken cancellationToken = default)
        {
            if (roadMapFilter is null || roadMapFilter.CountRows <= 0 || roadMapFilter.Page <= 0)
                return new ResultPage<RoadMap>() { Count = 0, Values = new List<RoadMap>() };

            var query = @" select * from dbo.RoadMap where IsDeleted = 0";
            var queryCount = @" select COUNT(*) from dbo.RoadMap where IsDeleted = 0";

            dynamic filter = new ExpandoObject();

            if (roadMapFilter.Id != null)
            {
                query += " AND Id = @Id";
                queryCount += " AND Id = @Id";

                filter.Id = roadMapFilter.Id;
            }

            if (!string.IsNullOrWhiteSpace(roadMapFilter.SearchString))
            {
                query += " AND CONTAINS(SearchString, @SearchString)";
                queryCount += " AND CONTAINS(SearchString, @SearchString)";

                var values = (roadMapFilter.SearchString ?? string.Empty).Split(" ").Select(value => value.Trim()).Where(value => !string.IsNullOrWhiteSpace(value)).Select(value => $"\"{value}*\"");

                filter.SearchString = $"({string.Join(" AND ", values)})";
            }

            query += @$"
                ORDER BY Id
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY;

                {queryCount}
            ";

            filter.Offset = (roadMapFilter.Page - 1) * roadMapFilter.CountRows;
            filter.PageSize = roadMapFilter.CountRows;

            var results = await _session.Connection.QueryMultipleAsync(new CommandDefinition(query, filter as object, transaction: _session.Transaction, cancellationToken: cancellationToken));
            var rowsDapper = results.Read().ToList();
            var count = results.Read<long>().Single();

            if (rowsDapper is null)
                return new ResultPage<RoadMap>() { Count = 0, Values = new List<RoadMap>() };

            var sales = new List<RoadMap>();

            foreach (var row in rowsDapper)
                sales.Add(await Map(row));

            return new ResultPage<RoadMap>() { Count = count, Values = sales };
        }

        public async Task<RoadMap> Update(RoadMap entity, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.RoadMap
                SET 
                    Date = @Date,
                    DriverId = @DriverId,
                    SearchString = @SearchString,
                    UpdateDate = SYSUTCDATETIME(),
                    Version = Version + 1
                WHERE Id = @Id AND IsDeleted = 0;
            ";

            entity.SearchString = entity.ToString();
            await _session.Connection.ExecuteAsync(new CommandDefinition(query, new { DriverId = entity.Driver?.Id, entity.SearchString, entity.Date, entity.Id}, transaction: _session.Transaction, cancellationToken: cancellation));
            await _roadMapSaleRepository.SyncCollection(entity, entity.Sales);

            return await Get(entity.Id, cancellation);
        }

        public async Task OnTrip(RoadMap entity, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.RoadMap
                SET 
                    State = @State, 
                    TravelDate = SYSUTCDATETIME(),
                    UpdateDate = SYSUTCDATETIME(),
                    Version = Version + 1
                WHERE Id = @Id AND IsDeleted = 0;
            ";
            await _session.Connection.ExecuteAsync(new CommandDefinition(query, new { entity.Id, State = RoadMapState.EnViaje }, transaction: _session.Transaction, cancellationToken: cancellation));
        }
        
        public async Task FinishTrip(RoadMap entity, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.RoadMap
                SET 
                    State = @State, 
                    CompletionDate = SYSUTCDATETIME(),
                    UpdateDate = SYSUTCDATETIME(),
                    Version = Version + 1
                WHERE Id = @Id AND IsDeleted = 0;
            ";
            await _session.Connection.ExecuteAsync(new CommandDefinition(query, new { entity.Id, State = RoadMapState.Finalizado }, transaction: _session.Transaction, cancellationToken: cancellation));
        }

        public async Task<RoadMap?> Map(dynamic? rowDapper, CancellationToken cancellation = default)
        {
            if (rowDapper is null)
                return null;

            var roadMap = new RoadMap();
            roadMap.Id = rowDapper.Id;
            roadMap.SearchString = rowDapper.SearchString;
            roadMap.CreationDate = rowDapper.CreationDate;
            roadMap.RemoveDate = rowDapper.RemoveDate;
            roadMap.UpdateDate = rowDapper.UpdateDate;
            roadMap.Version = rowDapper.Version;
            roadMap.Guid = rowDapper.Guid;
            roadMap.IsDeleted = rowDapper.IsDeleted;
            roadMap.Number = rowDapper.Number;
            roadMap.Date = rowDapper.Date;
            roadMap.TravelDate = rowDapper.TravelDate;
            roadMap.CompletionDate = rowDapper.CompletionDate;
            roadMap.State = Enum.Parse<RoadMapState>(rowDapper.State);
            roadMap.Sales = await _roadMapSaleRepository.Get(roadMap, cancellation);

            if (rowDapper.DriverId != null)
                roadMap.Driver = await _driverRepository.Get((long)rowDapper.DriverId, cancellation);

            return roadMap;
        }
    }
}
