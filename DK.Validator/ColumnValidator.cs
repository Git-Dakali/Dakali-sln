using DK.Domain.Locations;
using DK.Repositories.Locations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Validator
{
    public class ColumnValidator
    {
        private ColumnRepository _columnRepository;

        public ColumnValidator(ColumnRepository columnRepository)
        {
            _columnRepository = columnRepository ?? throw new ArgumentNullException("ColumnRepository");
        }

        public async Task Create(Column column, CancellationToken cancellationToken = default)
        {
            await Code(column, cancellationToken);
            await Name(column, cancellationToken);
        }

        public async Task Update(Column column, CancellationToken cancellationToken = default)
        {
            if (!(await Exist(column, cancellationToken)))
                throw new Exception($"No existe la columna {column.Code}-{column.Name}");

            await Code(column, cancellationToken);
            await Name(column, cancellationToken);
        }

        public async Task Delete(Column column, CancellationToken cancellationToken = default)
        {
            if (!(await Exist(column, cancellationToken)))
                throw new Exception($"No existe la columna {column.Code}-{column.Name}");
        }

        public async Task<bool> Exist(Column column, CancellationToken cancellationToken = default)
        {
            return (await _columnRepository.Get(column.Id, cancellationToken)) != null;
        }

        public async Task Code(Column column, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(column.Code))
                throw new Exception("El codigo esta vacío.");

            if (column.Id > 0)
                return;

            var persisted = await _columnRepository.Get(column.Code, cancellationToken);

            if (persisted != null)
                throw new Exception($"El codigo {column.Code} ya existe.");
        }

        public async Task Name(Column column, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(column.Name))
                throw new Exception("El nombre esta vacío.");
        }
    }
}
