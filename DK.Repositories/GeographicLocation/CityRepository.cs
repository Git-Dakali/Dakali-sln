using Dakali.Domine;
using Dakali.Interface.Connection;
using Dapper;
using DK.Domain.GeographicLocation;
using DK.Repositories.Interface.Base;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Repositories.GeographicLocation
{
    public class CityRepository : IRepositoryCode<City>
    {
        private ISession _session;
        private ProvinceRepository _provinceRepository;

        public CityRepository(ISession session, ProvinceRepository provinceRepository)
        {
            _session = session;
            _provinceRepository = provinceRepository;
        }

        public async Task<IEnumerable<City>> GetAll(CancellationToken cancellation = default)
        {
            var query = "select * from dbo.City where IsDeleted = 0";
            var rows = await _session.Connection.QueryAsync(query, new { }, transaction: _session.Transaction);

            if (rows is null)
                return Enumerable.Empty<City>();

            var citys = new List<City>();

            foreach (var row in rows)
                citys.Add(await Map(row));

            return citys;
        }

        public async Task<ResultPage<City>> GetPage(CityFilter cityFilter, CancellationToken cancellationToken = default)
        {
            if (cityFilter is null || cityFilter.CountRows <= 0 || cityFilter.Page <= 0)
                return new ResultPage<City>() { Count = 0, Values = new List<City>() };

            var query = "SELECT * FROM dbo.City where IsDeleted = 0 ";
            var queryCount = "SELECT COUNT(*) FROM dbo.City where IsDeleted = 0 ";
            dynamic filter = new ExpandoObject();

            if (cityFilter.Id != null)
            {
                query += " AND Id = @Id";
                queryCount += " AND Id = @Id";

                filter.Id = cityFilter.Id;
            }

            if (!string.IsNullOrWhiteSpace(cityFilter.SearchString))
            {
                query += " AND SearchString like @SearchString ";
                queryCount += " AND SearchString like @SearchString ";

                filter.SearchString = $"%{cityFilter.SearchString}%";
            }

            if (!string.IsNullOrWhiteSpace(cityFilter.Code))
            {
                query += " AND Code = @Code";
                queryCount += " AND Code = @Code";

                filter.Code = cityFilter.Code;
            }

            if (cityFilter.ProvinceId != null)
            {
                query += " AND ProvinceId = @ProvinceId";
                queryCount += " AND ProvinceId = @ProvinceId";
                filter.ProvinceId = cityFilter.ProvinceId;
            }

            query += @$"
                ORDER BY Id
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY;

                {queryCount}
            ";

            filter.Offset = (cityFilter.Page - 1) * cityFilter.CountRows;
            filter.PageSize = cityFilter.CountRows;



            var results = await _session.Connection.QueryMultipleAsync(query, filter as object, transaction: _session.Transaction);
            var rows = results.Read().ToList();
            var count = results.Read<long>().Single();

            if (rows is null)
                return new ResultPage<City>() { Count = 0, Values = new List<City>() };

            var citys = new List<City>();

            foreach (var row in rows)
                citys.Add(await Map(row));

            return new ResultPage<City>() { Count = count, Values = citys};
        }

        public async Task<City> Get(long id, CancellationToken cancellation = default)
        {
            var query = "select * from dbo.City where IsDeleted = 0 AND Id = @Id";
            var row = await _session.Connection.QuerySingleOrDefaultAsync(query, new { Id = id }, transaction: _session.Transaction);

            if (row is null)
                return null;

            return await Map(row);
        }

        public async Task<City> Get(string zipCode, CancellationToken cancellation = default)
        {
            var query = "select * from dbo.City where IsDeleted = 0 AND ZipCode = @ZipCode";
            var row = await _session.Connection.QueryFirstOrDefaultAsync(query, new { ZipCode = zipCode }, transaction: _session.Transaction);

            if (row is null)
                return null;

            return await Map(row);
        }

        public async Task<IEnumerable<City>> Get(Province province, CancellationToken cancellation = default)
        {
            var query = "select * from dbo.City where IsDeleted = 0 and ProvinceId = @ProvinceId";
            var rows = await _session.Connection.QueryAsync(query, new { ProvinceId = province.Id }, transaction: _session.Transaction);

            if (rows is null)
                return Enumerable.Empty<City>();

            var citys = new List<City>();

            foreach (var row in rows)
                citys.Add(await Map(row));

            return citys;
        }

        public async Task<City> Create(City entity, CancellationToken cancellation = default)
        {
            var query = @"
            INSERT INTO dbo.City (ZipCode, Name, ProvinceId, SearchString)
            OUTPUT INSERTED.*
            VALUES (@ZipCode, @Name, @ProvinceId, @SearchString);";

            entity.SearchString = entity.ToString();
            var row = await _session.Connection.QuerySingleAsync(query, new { entity.ZipCode, entity.Name, entity.SearchString, ProvinceId = entity.Province.Id}, transaction: _session.Transaction);

            if (row is null)
                return null;

            return await Map(row);
        }

        public async Task<City> Update(City entity, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.City
                SET 
                    ZipCode = @ZipCode,
                    Name = @Name,
                    ProvinceId = @ProvinceId,
                    SearchString = @SearchString,
                    UpdateDate = SYSUTCDATETIME(),
                    Version = Version + 1
                OUTPUT INSERTED.*
                WHERE Id = @Id AND IsDeleted = 0;
            ";

            entity.SearchString = entity.ToString();
            var row = await _session.Connection.QuerySingleAsync(query, new { entity.Id, entity.ZipCode, entity.Name, entity.SearchString, ProvinceId = entity.Province.Id}, transaction: _session.Transaction);

            if (row is null)
                throw new Exception($"La Localidad {entity.ZipCode}-{entity.Name} no existe para actualizar.");

            return await Map(row);
        }

        public async Task Delete(City entity, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.City
                   SET IsDeleted = 1,
                       RemoveDate = SYSUTCDATETIME(),
                       UpdateDate = SYSUTCDATETIME(),
                       Version = Version + 1
                 WHERE Id = @id AND IsDeleted = 0;";

            await _session.Connection.ExecuteAsync(query, entity, transaction: _session.Transaction);
        }

        public async Task<City> Map(dynamic rowDapper, CancellationToken cancellation = default)
        {
            var city = new City();
            city.Id = rowDapper.Id;
            city.SearchString = rowDapper.SearchString;
            city.ZipCode = rowDapper.ZipCode;
            city.CreationDate = rowDapper.CreationDate;
            city.RemoveDate = rowDapper.RemoveDate;
            city.UpdateDate = rowDapper.UpdateDate;
            city.Version = rowDapper.Version;
            city.Guid = rowDapper.Guid;
            city.IsDeleted = rowDapper.IsDeleted;
            city.Name = rowDapper.Name;
            city.Province = await _provinceRepository.Get((long)rowDapper.ProvinceId, cancellation);

            return city;
        }
    }
}
