using Dakali.Interface.Connection;
using Dapper;
using DK.Domain.GeographicLocation;
using DK.Repositories.Interface.Base;
using System;
using System.Collections.Generic;
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
            {
                var city = new City();
                city.Id = row.Id;
                city.SearchString = row.SearchString;
                city.ZipCode = row.ZipCode;
                city.CreationDate = row.CreationDate;
                city.RemoveDate = row.RemoveDate;
                city.UpdateDate = row.UpdateDate;
                city.Version = row.Version;
                city.Guid = row.Guid;
                city.IsDeleted = row.IsDeleted;
                city.Name = row.Name;
                city.Province = await _provinceRepository.Get((long)row.ProvinceId, cancellation);

                citys.Add(city);
            }

            return citys;
        }

        public async Task<City> Get(long id, CancellationToken cancellation = default)
        {
            var query = "select * from dbo.City where IsDeleted = 0 AND Id = @Id";
            var row = await _session.Connection.QuerySingleOrDefaultAsync(query, new { Id = id }, transaction: _session.Transaction);

            if (row is null)
                return null;

            var city = new City();
            city.Id = row.Id;
            city.SearchString = row.SearchString;
            city.ZipCode = row.ZipCode;
            city.CreationDate = row.CreationDate;
            city.RemoveDate = row.RemoveDate;
            city.UpdateDate = row.UpdateDate;
            city.Version = row.Version;
            city.Guid = row.Guid;
            city.IsDeleted = row.IsDeleted;
            city.Name = row.Name;
            city.Province = await _provinceRepository.Get((long)row.ProvinceId, cancellation);

            return city;
        }

        public async Task<City> Get(string code, CancellationToken cancellation = default)
        {
            var query = "select * from dbo.City where IsDeleted = 0 AND Code = @Code";
            var row = await _session.Connection.QuerySingleOrDefaultAsync(query, new { Code = code }, transaction: _session.Transaction);

            if (row is null)
                return null;

            var city = new City();
            city.Id = row.Id;
            city.SearchString = row.SearchString;
            city.ZipCode = row.ZipCode;
            city.CreationDate = row.CreationDate;
            city.RemoveDate = row.RemoveDate;
            city.UpdateDate = row.UpdateDate;
            city.Version = row.Version;
            city.Guid = row.Guid;
            city.IsDeleted = row.IsDeleted;
            city.Name = row.Name;
            city.Province = await _provinceRepository.Get((long)row.ProvinceId, cancellation);

            return city;
        }

        public async Task<IEnumerable<City>> Get(Province province, CancellationToken cancellation = default)
        {
            var query = "select * from dbo.City where IsDeleted = 0 and ProvinceId = @ProvinceId";
            var rows = await _session.Connection.QueryAsync(query, new { ProvinceId = province.Id }, transaction: _session.Transaction);

            if (rows is null)
                return Enumerable.Empty<City>();

            var citys = new List<City>();

            foreach (var row in rows)
            {
                var city = new City();
                city.Id = row.Id;
                city.SearchString = row.SearchString;
                city.ZipCode = row.ZipCode;
                city.CreationDate = row.CreationDate;
                city.RemoveDate = row.RemoveDate;
                city.UpdateDate = row.UpdateDate;
                city.Version = row.Version;
                city.Guid = row.Guid;
                city.IsDeleted = row.IsDeleted;
                city.Name = row.Name;
                city.Province = await _provinceRepository.Get((long)row.ProvinceId, cancellation);

                citys.Add(city);
            }

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

            var city = new City();
            city.Id = row.Id;
            city.SearchString = row.SearchString;
            city.ZipCode = row.ZipCode;
            city.CreationDate = row.CreationDate;
            city.RemoveDate = row.RemoveDate;
            city.UpdateDate = row.UpdateDate;
            city.Version = row.Version;
            city.Guid = row.Guid;
            city.IsDeleted = row.IsDeleted;
            city.Name = row.Name;
            city.Province = await _provinceRepository.Get((long)row.ProvinceId, cancellation);

            return city;
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

            var city = new City();
            city.Id = row.Id;
            city.SearchString = row.SearchString;
            city.ZipCode = row.ZipCode;
            city.CreationDate = row.CreationDate;
            city.RemoveDate = row.RemoveDate;
            city.UpdateDate = row.UpdateDate;
            city.Version = row.Version;
            city.Guid = row.Guid;
            city.IsDeleted = row.IsDeleted;
            city.Name = row.Name;
            city.Province = await _provinceRepository.Get((long)row.ProvinceId, cancellation);

            return city;
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
    }
}
