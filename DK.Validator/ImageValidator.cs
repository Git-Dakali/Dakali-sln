using DK.Domain.Products;
using DK.Repositories.Products;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Validator
{
    public class ImageValidator
    {
        public StoredFileRepository _storedFileRepository;

        public ImageValidator(StoredFileRepository storedFileRepository) 
        {
            _storedFileRepository = storedFileRepository ?? throw new ArgumentNullException("StoredFileRepository");
        }

        public async Task Create(Image image, CancellationToken cancellationToken = default)
        {
            await File(image, cancellationToken);
        }

        public async Task Update(Image image, CancellationToken cancellationToken = default)
        {
            await File(image, cancellationToken);
        }

        public async Task Delete(Image image, CancellationToken cancellationToken = default)
        {
            
        }

        public async Task File(Image image, CancellationToken cancellationToken = default)
        {
            if (image.File is null)
                throw new Exception("el archivo esta vacion.");
            if (image.File.Id == 0)
                throw new Exception("el archivo no esta guardado.");

            var filePersisted = await _storedFileRepository.Get(image.File.Id, cancellationToken);

            if (filePersisted is null)
                throw new Exception("el archivo no existe o fue eliminado.");
        }
    }
}
