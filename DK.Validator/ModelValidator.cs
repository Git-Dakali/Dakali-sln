using DK.Domain.Products;
using DK.Repositories.Products;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Validator
{
    public class ModelValidator
    {
        public ModelRepository _modelRepository;
        public CategoryValidator _categoryValidator;

        public ModelValidator(ModelRepository modelRepository, CategoryValidator categoryValidator) 
        {
            _modelRepository = modelRepository ?? throw new ArgumentNullException("ModelRepository");
            _categoryValidator = categoryValidator ?? throw new ArgumentNullException("CategoryRepository");
        }

        public async Task Create(Model model, CancellationToken cancellationToken = default)
        {
            await Code(model, cancellationToken);
            await Category(model, cancellationToken);
            await FieldGroups(model, cancellationToken);
        }

        public async Task Update(Model model, CancellationToken cancellationToken = default)
        {
            if (!(await Exist(model, cancellationToken)))
                throw new Exception($"No existe el modelo {model.Id}-{model.Code}");

            await Code(model, cancellationToken);
            await Category(model, cancellationToken);
            await FieldGroups(model, cancellationToken);
        }

        public async Task Delete(Model model, CancellationToken cancellationToken = default)
        {
            if (!(await Exist(model, cancellationToken)))
                throw new Exception($"No existe el modelo {model.Id}-{model.Code}");

        }

        public async Task<bool> Exist(Model model, CancellationToken cancellationToken = default)
        {
            return (await _modelRepository.Get(model.Id, cancellationToken)) != null;
        }

        public async Task Code(Model model, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(model.Code))
                throw new Exception("El codigo esta vacío.");

            if (model.Id > 0)
                return;

            var modelPersisted = await _modelRepository.Get(model.Code, cancellationToken);

            if (modelPersisted != null)
                throw new Exception($"El codigo {model.Code} ya existe.");
        }

        public async Task Category(Model model, CancellationToken cancellationToken = default)
        {
            if (model.Category is null)
                throw new Exception("La categoria esta vacío.");

            if (!await _categoryValidator.Exist(model.Category, cancellationToken))
                throw new Exception("La categoria no existe.");
        }

        public async Task Sizes(Model model, CancellationToken cancellationToken = default)
        {
            if (model.VariantNames is null)
                throw new Exception("La lista de tamaños esta vacío.");

            if (!model.VariantNames.Any())
                throw new Exception("La lista de tamaños esta vacío.");

            var group = model.VariantNames.GroupBy(name => name);

            if (!group.Any(g => g.Count() > 1))
                return;

            var key = group.FirstOrDefault(g => g.Count() > 1).Key;
            throw new Exception($"Existen variantes duplicados {key}.");

        }

        public async Task FieldGroups(Model model, CancellationToken cancellationToken = default)
        {
            if (model.FieldGroups is null)
                throw new Exception("La lista de grupos esta vacío.");

            if (!model.FieldGroups.Any())
                throw new Exception("La lista de grupos esta vacío.");

            var group = model.FieldGroups.GroupBy(s => s.Name);

            if (group.Any(g => g.Count() > 1))
            {
                var key = group.FirstOrDefault(g => g.Count() > 1).Key;
                throw new Exception($"Existen grupo duplicados {key}.");
            }

            foreach (var fieldGroup in model.FieldGroups)
            {
                if(fieldGroup.Fields is null)
                    throw new Exception($"En el grupo {fieldGroup.Name}, la lista de campos esta vacío.");

                if (fieldGroup.Fields.Count() == 0)
                    throw new Exception($"En el grupo {fieldGroup.Name}, la lista de campos esta vacío.");

                var fields = fieldGroup.Fields.GroupBy(s => s.Name);

                if (fields.Any(g => g.Count() > 1))
                {
                    var key = fields.FirstOrDefault(g => g.Count() > 1).Key;
                    throw new Exception($"Para el grupo {fieldGroup.Name}, existen campos duplicados {key}.");
                }
            }
        }
    }
}
