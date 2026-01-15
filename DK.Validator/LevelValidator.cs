using DK.Domain.Locations;
using DK.Domain.Products;
using DK.Repositories.Locations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Validator
{
    public class LevelValidator
    {
        private LevelRepository _levelRepository;

        public LevelValidator(LevelRepository levelRepository)
        {
            _levelRepository = levelRepository ?? throw new ArgumentNullException("LevelRepository");
        }

        public async Task Create(Level level, CancellationToken cancellationToken = default)
        {
            await Code(level, cancellationToken);
            await Name(level, cancellationToken);
        }

        public async Task Update(Level level, CancellationToken cancellationToken = default)
        {
            if (!(await Exist(level, cancellationToken)))
                throw new Exception($"No existe el nivel {level.Code}-{level.Name}");

            await Code(level, cancellationToken);
            await Name(level, cancellationToken);
        }

        public async Task Delete(Level level, CancellationToken cancellationToken = default)
        {
            if (!(await Exist(level, cancellationToken)))
                throw new Exception($"No existe el nivel {level.Code}-{level.Name}");
        }

        public async Task<bool> Exist(Level level, CancellationToken cancellationToken = default)
        {
            return (await _levelRepository.Get(level.Id, cancellationToken)) != null;
        }

        public async Task Code(Level level, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(level.Code))
                throw new Exception("El codigo esta vacío.");

            if (level.Id > 0)
                return;

            var persisted = await _levelRepository.Get(level.Code, cancellationToken);

            if (persisted != null)
                throw new Exception($"El codigo {level.Code} ya existe.");
        }

        public async Task Name(Level level, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(level.Name))
                throw new Exception("El nombre esta vacío.");
        }
    }
}
