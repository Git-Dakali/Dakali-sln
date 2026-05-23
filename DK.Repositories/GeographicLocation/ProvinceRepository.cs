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
    public class ProvinceRepository : IRepositoryCode<Province>
    {
        private ISession _session;
        private CountryRepository _countryRepository;

        public ProvinceRepository(ISession session, CountryRepository countryRepository)
        {
            _session = session;
            _countryRepository = countryRepository;
        }

        public async Task<IEnumerable<Province>> GetAll(CancellationToken cancellation = default)
        {
            var query = "select * from dbo.Province where IsDeleted = 0";
            var rows = await _session.Connection.QueryAsync(new CommandDefinition(query, new { }, transaction: _session.Transaction, cancellationToken: cancellation));

            if (rows is null)
                return Enumerable.Empty<Province>();

            var provinces = new List<Province>();

            foreach (var row in rows)
            {
                var province = new Province();
                province.Id = row.Id;
                province.SearchString = row.SearchString;
                province.Code = row.Code;
                province.CreationDate = row.CreationDate; 
                province.RemoveDate = row.RemoveDate; 
                province.UpdateDate = row.UpdateDate; 
                province.Version = row.Version; 
                province.Guid = row.Guid; 
                province.IsDeleted = row.IsDeleted; 
                province.Name = row.Name;
                province.Country = await _countryRepository.Get((long)row.CountryId, cancellation);
                
                provinces.Add(province);
            }

            return provinces;
        }

        public async Task<IEnumerable<Province>> Get(Country country, CancellationToken cancellation = default)
        {
            var query = "select * from dbo.Province where IsDeleted = 0 AND CountryId = @CountryId";
            var rows = await _session.Connection.QueryAsync(new CommandDefinition(query, new { CountryId = country.Id}, transaction: _session.Transaction, cancellationToken: cancellation));

            if (rows is null)
                return Enumerable.Empty<Province>();

            var provinces = new List<Province>();

            foreach (var row in rows)
            {
                var province = new Province();
                province.Id = row.Id;
                province.SearchString = row.SearchString;
                province.Code = row.Code;
                province.CreationDate = row.CreationDate;
                province.RemoveDate = row.RemoveDate;
                province.UpdateDate = row.UpdateDate;
                province.Version = row.Version;
                province.Guid = row.Guid;
                province.IsDeleted = row.IsDeleted;
                province.Name = row.Name;
                province.Country = await _countryRepository.Get((long)row.CountryId, cancellation);

                provinces.Add(province);
            }

            return provinces;
        }

        public async Task<Province> Get(long id, CancellationToken cancellation = default)
        {
            var query = "select * from dbo.Province where IsDeleted = 0 AND Id = @Id";
            var row = await _session.Connection.QuerySingleOrDefaultAsync(new CommandDefinition(query, new { Id = id }, transaction: _session.Transaction, cancellationToken: cancellation));

            if (row is null)
                return null;

            var province = new Province();
            province.Id = row.Id;
            province.SearchString = row.SearchString;
            province.Code = row.Code;
            province.CreationDate = row.CreationDate;
            province.RemoveDate = row.RemoveDate;
            province.UpdateDate = row.UpdateDate;
            province.Version = row.Version;
            province.Guid = row.Guid;
            province.IsDeleted = row.IsDeleted;
            province.Name = row.Name;
            province.Country = await _countryRepository.Get((long)row.CountryId, cancellation);

            return province;
        }

        public async Task<Province> Get(string code, CancellationToken cancellation = default)
        {
            var query = "select * from dbo.Province where IsDeleted = 0 AND Code = @Code";
            var row = await _session.Connection.QuerySingleOrDefaultAsync(new CommandDefinition(query, new { Code = code }, transaction: _session.Transaction, cancellationToken: cancellation));

            if (row is null)
                return null;

            var province = new Province();
            province.Id = row.Id;
            province.SearchString = row.SearchString;
            province.Code = row.Code;
            province.CreationDate = row.CreationDate;
            province.RemoveDate = row.RemoveDate;
            province.UpdateDate = row.UpdateDate;
            province.Version = row.Version;
            province.Guid = row.Guid;
            province.IsDeleted = row.IsDeleted;
            province.Name = row.Name;
            province.Country = await _countryRepository.Get((long)row.CountryId, cancellation);

            return province;
        }

        public async Task<Province> Create(Province entity, CancellationToken cancellation = default)
        {
            var query = @"
            INSERT INTO dbo.Province (Code, Name, SearchString, CountryId)
            OUTPUT INSERTED.*
            VALUES (@Code, @Name, @SearchString, @CountryId);";

            entity.SearchString = entity.ToString();
            var row = await _session.Connection.QuerySingleAsync(new CommandDefinition(query, new { entity.Code, entity.Name, entity.SearchString, CountryId = entity.Country.Id }, transaction: _session.Transaction, cancellationToken: cancellation));

            if (row is null)
                return null;

            var province = new Province();
            province.Id = row.Id;
            province.SearchString = row.SearchString;
            province.Code = row.Code;
            province.CreationDate = row.CreationDate;
            province.RemoveDate = row.RemoveDate;
            province.UpdateDate = row.UpdateDate;
            province.Version = row.Version;
            province.Guid = row.Guid;
            province.IsDeleted = row.IsDeleted;
            province.Name = row.Name;
            province.Country = await _countryRepository.Get((long)row.CountryId, cancellation);

            return province;
        }

        public async Task<Province> Update(Province entity, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.Province
                SET 
                    Name = @Name,
                    CountryId = @CountryId,
                    SearchString = @SearchString,
                    UpdateDate = SYSUTCDATETIME(),
                    Version = Version + 1
                OUTPUT INSERTED.*
                WHERE Id = @Id AND IsDeleted = 0;
            ";

            entity.SearchString = entity.ToString();
            var row = await _session.Connection.QuerySingleAsync(new CommandDefinition(query, new { entity.Id, entity.Name, entity.SearchString, CountryId = entity.Country.Id}, transaction: _session.Transaction, cancellationToken: cancellation));

            if (row is null)
                throw new Exception($"La Provincia {entity.Code}-{entity.Name} no existe para actualizar.");

            var province = new Province();
            province.Id = row.Id;
            province.SearchString = row.SearchString;
            province.Code = row.Code;
            province.CreationDate = row.CreationDate;
            province.RemoveDate = row.RemoveDate;
            province.UpdateDate = row.UpdateDate;
            province.Version = row.Version;
            province.Guid = row.Guid;
            province.IsDeleted = row.IsDeleted;
            province.Name = row.Name;
            province.Country = await _countryRepository.Get((long)row.CountryId, cancellation);

            return province;
        }

        public async Task Delete(Province entity, CancellationToken cancellation = default)
        {
            var query = @"
                UPDATE dbo.Province
                   SET IsDeleted = 1,
                       RemoveDate = SYSUTCDATETIME(),
                       UpdateDate = SYSUTCDATETIME(),
                       Version = Version + 1
                 WHERE Id = @id AND IsDeleted = 0;";

            await _session.Connection.ExecuteAsync(new CommandDefinition(query, entity, transaction: _session.Transaction, cancellationToken: cancellation));
        }
    }
}
