using Dakali.Interface.Connection;
using Dapper;
using DK.Domain.RoadMaps;
using DK.Domain.Sales;
using DK.Repositories.Interface.Base;
using System;
using System.Collections.Generic;
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
            var rowDapper = await _session.Connection.QuerySingleAsync(query, new { entity.Date, DriverId = entity.Driver?.Id, State = RoadMapState.Creado.ToString(), entity.SearchString }, transaction: _session.Transaction);
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

            await _session.Connection.ExecuteAsync(query, entity, transaction: _session.Transaction);
        }

        public async Task<RoadMap> Get(long id, CancellationToken cancellation = default)
        {
            var query = "select * from dbo.RoadMap where IsDeleted = 0 AND Id = @Id";
            var rowDapper = await _session.Connection.QuerySingleOrDefaultAsync(query, new { Id = id }, transaction: _session.Transaction);
            
            return await Map(rowDapper, cancellation);
        }

        public async Task<RoadMap> Get(Sale sale, CancellationToken cancellation = default)
        {
            var query = "select * from dbo.RoadMap r where r.IsDeleted = 0 AND r.Id in (SELECT rs.RoadMapId FROM dbo.RoadMapSale rs WHERE rs.SaleId = @Id)";
            
            var rowDapper = await _session.Connection.QuerySingleOrDefaultAsync(query, new { sale.Id }, transaction: _session.Transaction);

            return await Map(rowDapper, cancellation);
        }

        public async Task<IEnumerable<RoadMap>> GetAll(CancellationToken cancellation = default)
        {
            var query = "select * from dbo.RoadMap where IsDeleted = 0";
            var rowsDapper = await _session.Connection.QueryAsync(query, new { }, transaction: _session.Transaction);

            var roadMaps = new List<RoadMap>();
            foreach (var row in rowsDapper)
                roadMaps.Add(await Map(row, cancellation));

            return roadMaps;
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
                OUTPUT INSERTED.*
                WHERE Id = @Id AND IsDeleted = 0;
            ";

            entity.SearchString = entity.ToString();
            var rowDapperUpdated = await _session.Connection.QuerySingleAsync(query, new { DriverId = entity.Driver?.Id, entity.SearchString, entity.Date, entity.Id}, transaction: _session.Transaction);
            
            if (rowDapperUpdated is null)
                throw new KeyNotFoundException($"La Hoja de Ruta {entity.Number} no se encontro para actualizar.");


            var roadMap = await Map(rowDapperUpdated, cancellation);
            await _roadMapSaleRepository.SyncCollection(roadMap, entity.Sales);

            return roadMap;
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
