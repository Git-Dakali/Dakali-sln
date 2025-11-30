using DK.Domain.Products;
using DK.Repositories.Products;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DK.Validator
{
    public class VariantValidator
    {
        public VariantRepository _variantRepository;

        public VariantValidator(VariantRepository variantRepository)
        {
            _variantRepository = variantRepository ?? throw new ArgumentNullException("VariantRepository");
        }

        public async Task Create(Product product, Variant variant, CancellationToken cancellationToken = default)
        {
            await Size(variant, cancellationToken);
            await Price(variant, cancellationToken);
            await SalePrice(variant, cancellationToken);
            await Colors(variant, cancellationToken);
            await Attributes(product, variant, cancellationToken);
        }

        public async Task Update(Product product, Variant variant, CancellationToken cancellationToken = default)
        {
            await Size(variant, cancellationToken);
            await Price(variant, cancellationToken);
            await SalePrice(variant, cancellationToken);
            await Colors(variant, cancellationToken);
            await Attributes(product, variant, cancellationToken);
        }

        public async Task Delete(Product product, Variant variant, CancellationToken cancellationToken = default)
        {
            if (!(await Exist(product, variant, cancellationToken)))
                throw new Exception($"No existe la variante de tamaño {variant.Name}");
        }

        public async Task<bool> Exist(Product product, Variant variant, CancellationToken cancellationToken = default)
        {
            return (await _variantRepository.Get(product, variant.Id, cancellationToken)) != null;
        }

        public async Task Size(Variant variant, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(variant.Name))
                throw new Exception("El tamaño esta vacio.");
        }

        public async Task Price(Variant variant, CancellationToken cancellationToken = default)
        {
            if (variant.Price < 0)
                throw new Exception("El precio no puede ser menor a cero.");
        }

        public async Task SalePrice(Variant variant, CancellationToken cancellationToken = default)
        {
            if (variant.SalePrice < 0)
                throw new Exception("El precio no puede ser menor a cero.");
        }

        public async Task Colors(Variant variant, CancellationToken cancellationToken = default)
        {
            if (variant.ColorsHex is null)
                throw new Exception("Se debe asignar un color.");
            if (variant.ColorsHex.Count() == 0)
                throw new Exception("Se debe asignar un color.");
        }

        public async Task Attributes(Product product, Variant variant, CancellationToken cancellationToken = default)
        {
            if (variant.PropertyGroups is null)
                throw new Exception("No tiene configurado los grupos.");

            if (variant.PropertyGroups.Count() == 0)
                throw new Exception("No tiene configurado los grupos.");

            var groups = variant.PropertyGroups.ToDictionary(x => x.Name.ToUpper());

            foreach (var fieldGroup in product.Model.FieldGroups)
            {
                
                if (!groups.ContainsKey(fieldGroup.Name.ToUpper()))
                    throw new Exception($"No tiene configurado el grupo {fieldGroup.Name}");

                var properties = groups[fieldGroup.Name.ToUpper()].Properties.ToDictionary(a => a.Field.ToUpper());

                foreach (var field in fieldGroup.Fields)
                {
                    if(properties.ContainsKey(field.Name.ToUpper()))
                        throw new Exception($"No tiene configurado el campo {field.Name}.");
                }
            }
        }
    }
}
