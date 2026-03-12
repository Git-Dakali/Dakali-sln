using Dakali.Interface.Connection;
using Dapper;
using DK.Domain.Locations;
using DK.Domain.Products;
using DK.Repositories.Interface.Base;
using DK.Repositories.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Repositories.Locations
{
    public class LocationRepository : IRepository<Location>
    {
        private ISession _session;
        private HallwayRepository _hallwayRepository;
        private ColumnRepository _columnRepository;
        private LevelRepository _levelRepository;
        private LocationStateRepository _locationStateRepository;

        public LocationRepository(ISession session, HallwayRepository hallwayRepository, ColumnRepository columnRepository, LevelRepository levelRepository, LocationStateRepository locationStateRepository)
        {
            _session = session;
            _columnRepository = columnRepository;
            _levelRepository = levelRepository;
            _locationStateRepository = locationStateRepository;
            _hallwayRepository = hallwayRepository;
        }

        public async Task<Location> Create(Location entity, CancellationToken cancellation = default)
        {
            var query = @"
            INSERT INTO dbo.Location (HallwayId, ColumnId, LevelId, LocationStateId, SearchString)
            OUTPUT INSERTED.*
            VALUES (@HallwayId, @ColumnId, @LevelId, @LocationStateId, @SearchString);";

            entity.SearchString = entity.ToString();
            var rowDapper = await _session.Connection.QuerySingleAsync(query,
                new
                {
                    HallwayId = entity.Hallway.Id,
                    ColumnId = entity.Column.Id,
                    LevelId = entity.Level.Id,
                    LocationStateId = entity.State.Id,
                    entity.SearchString
                }, transaction: _session.Transaction);

            if (rowDapper == null)
                return null;

            return await Map(rowDapper);
        }

        public async Task Delete(Location entity, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.Location
                   SET IsDeleted = 1,
                       RemoveDate = SYSUTCDATETIME(),
                       UpdateDate = SYSUTCDATETIME(),
                       Version = Version + 1
                 WHERE Id = @id AND IsDeleted = 0;";

            await _session.Connection.ExecuteAsync(query, entity, transaction: _session.Transaction);
        }

        public async Task<Location> Get(long id, CancellationToken cancellation = default)
        {
            var query = @"
                select *
                from dbo.Location 
                where IsDeleted = 0 AND Id = @Id";
            var rowDapper = await _session.Connection.QuerySingleOrDefaultAsync<dynamic>(query, new { Id = id }, transaction: _session.Transaction);

            if (rowDapper == null)
                return null;

            return await Map(rowDapper);
        }

        public async Task<Location> Get(string hallwayCode, string columnCode, string levelCode, CancellationToken cancellation = default)
        {
            var query = @"
                select *
                from dbo.Location 
                Where IsDeleted = 0 
                    AND HallwayId = (select h.Id From Hallway h Where h.IsDeleted = 0 AND h.Code = @hallwayCode)
                    AND ColumnId = (select c.Id From LocationColumn c Where c.IsDeleted = 0 AND c.Code = @columnCode)
                    AND LevelId = (select n.Id From Level n Where n.IsDeleted = 0 AND n.Code = @levelCode) ";
            var rowDapper = await _session.Connection.QuerySingleOrDefaultAsync<dynamic>(query, new { hallwayCode, columnCode, levelCode }, transaction: _session.Transaction);

            if (rowDapper == null)
                return null;

            return await Map(rowDapper);
        }

        public async Task<Location> Get(Hallway hallway, Column column, Level level, CancellationToken cancellation = default)
        {
            var query = @"
                select *
                from dbo.Location 
                where IsDeleted = 0 AND HallwayId = @HallwayId AND ColumnId = @ColumnId AND LevelId = @LevelId";
            var rowDapper = await _session.Connection.QuerySingleOrDefaultAsync<dynamic>(query, new { HallwayId = hallway.Id, ColumnId = column.Id, LevelId = level.Id }, transaction: _session.Transaction);

            if (rowDapper == null)
                return null;

            return await Map(rowDapper);
        }

        public async Task<IEnumerable<Location>> GetAll(CancellationToken cancellation = default)
        {
            var query = @"
                select *
                from dbo.Location 
                where IsDeleted = 0";
            var rowsDapper = await _session.Connection.QueryAsync(query, new { }, transaction: _session.Transaction);

            if (rowsDapper == null)
                return Enumerable.Empty<Location>();

            var locations = new List<Location>();
            
            foreach (var rowDapper in rowsDapper)
                locations.Add(await Map(rowDapper));
            
            return locations;
        }

        public async Task<Location> Update(Location entity, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.Location
                SET 
                    HallwayId = @HallwayId,
                    ColumnId = @ColumnId,
                    LevelId = @LevelId,
                    LocationStateId = @LocationStateId,
                    SearchString = @SearchString,
                    UpdateDate = SYSUTCDATETIME(),
                    Version = Version + 1
                OUTPUT INSERTED.*
                WHERE Id = @Id AND IsDeleted = 0;
            ";

            entity.SearchString = entity.ToString();
            await _session.Connection.QuerySingleAsync<Model>(query, new
            {
                entity.Id,
                LocationStateId = entity.State.Id,
                HallwayId = entity.Hallway.Id,
                ColumnId = entity.Column.Id,
                LevelId = entity.Level.Id,
                entity.SearchString
            }, transaction: _session.Transaction);

            return await Get(entity.Id, cancellation) ?? throw new KeyNotFoundException($"La Ubicacion {entity.Hallway.ToString()}-{entity.Column.ToString()}-{entity.Level.ToString()} no se encontro para actualizar.");
        }

        public async Task<Location> Map(dynamic rowDapper, CancellationToken cancellation = default)
        {
            var newLocation = new Location();
            newLocation.Id = rowDapper.Id;
            newLocation.SearchString = rowDapper.SearchString;
            newLocation.CreationDate = rowDapper.CreationDate;
            newLocation.UpdateDate = rowDapper.UpdateDate;
            newLocation.RemoveDate = rowDapper.RemoveDate;
            newLocation.IsDeleted = rowDapper.IsDeleted;
            newLocation.Guid = rowDapper.Guid;
            newLocation.State = await _locationStateRepository.Get(rowDapper.LocationStateId, cancellation);
            newLocation.Hallway = await _hallwayRepository.Get(rowDapper.HallwayId, cancellation);
            newLocation.Column = await _columnRepository.Get(rowDapper.ColumnId, cancellation);
            newLocation.Level = await _levelRepository.Get(rowDapper.LevelId, cancellation);

            return newLocation;
        }
    }
}
