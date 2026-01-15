using DK.Domain.Locations;
using DK.Repositories.Locations;
using DK.Validator;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Process.Locations
{
    public class ColumnProcess
    {
        private ColumnRepository _columnRepository;
        private ColumnValidator _columnValidator;
        
        public ColumnProcess(ColumnRepository columnRepository, ColumnValidator columnValidator)
        {
            _columnRepository = columnRepository;
            _columnValidator = columnValidator;
        }

        public async Task<IEnumerable<Column>> GetAll(CancellationToken cancellationToken = default)
        {
            return await _columnRepository.GetAll(cancellationToken);
        }

        public async Task<Column> Get(long id, CancellationToken cancellationToken = default)
        {
            return await _columnRepository.Get(id, cancellationToken);
        }

        public async Task<Column> Get(string code, CancellationToken cancellationToken = default)
        {
            return await _columnRepository.Get(code, cancellationToken);
        }

        public async Task<Column> Create(Column entity, CancellationToken cancellationToken = default)
        {

            await _columnValidator.Create(entity, cancellationToken);

            return await _columnRepository.Create(entity, cancellationToken);
        }

        public async Task<Column> Update(Column entity, CancellationToken cancellationToken = default)
        {
            await _columnValidator.Update(entity, cancellationToken);

            return await _columnRepository.Update(entity, cancellationToken);
        }

        public async Task Delete(Column entity, CancellationToken cancellationToken = default)
        {
            await _columnValidator.Delete(entity, cancellationToken);
            await _columnRepository.Delete(entity, cancellationToken);
        }
    }
}
