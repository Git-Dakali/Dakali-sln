using DK.Domain.Products;
using DK.Repositories.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
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
            await Cost(variant, cancellationToken);
            await Colors(variant, cancellationToken);
            await Images(variant, cancellationToken);
            await Attributes(product, variant, cancellationToken);
        }

        public async Task Update(Product product, Variant variant, CancellationToken cancellationToken = default)
        {
            await Size(variant, cancellationToken);
            await Cost(variant, cancellationToken);
            await Colors(variant, cancellationToken);
            await Images(variant, cancellationToken);
            await Attributes(product, variant, cancellationToken);
        }

        public async Task Delete(Product product, Variant variant, CancellationToken cancellationToken = default)
        {
            if (!(await Exist(product, variant, cancellationToken)))
                throw new Exception($"No existe la variante de tamaño {variant.Size}");
        }

        public async Task<bool> Exist(Product product, Variant variant, CancellationToken cancellationToken = default)
        {
            return (await _variantRepository.Get(product, variant.Id, cancellationToken)) != null;
        }

        public async Task Size(Variant variant, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(variant.Size))
                throw new Exception("El tamaño esta vacio.");
        }

        public async Task Cost(Variant variant, CancellationToken cancellationToken = default)
        {
            if (variant.Cost < 0)
                throw new Exception("El precio no puede ser menor a cero.");
        }

        public async Task Colors(Variant variant, CancellationToken cancellationToken = default)
        {
            if (variant.ColorsHex is null)
                throw new Exception("Se debe asignar un color.");
            if (variant.ColorsHex.Count == 0)
                throw new Exception("Se debe asignar un color.");
        }

        public async Task Images(Variant variant, CancellationToken cancellationToken = default)
        {
            if (variant.Images is null)
                throw new Exception("Se debe asignar una imagen.");
            if (variant.Images.Count == 0)
                throw new Exception("Se debe asignar una imagen.");
        }

        public async Task Attributes(Product product, Variant variant, CancellationToken cancellationToken = default)
        {
            if (variant.Attributes is null)
                throw new Exception("No tiene configurado el detalle.");

            if (variant.Attributes.Count == 0)
                throw new Exception("No tiene configurado el detalle.");

            var fieldsAttribute = variant.Attributes.ToDictionary(x => x.Field.ToUpper());

            foreach (var fieldGroup in product.Model.FieldGroups)
            {
                foreach (var field in fieldGroup.Fields)
                {
                    if(fieldsAttribute.ContainsKey(field.Name.ToUpper()))
                        throw new Exception($"No tiene configurado el campo {field.Name}.");
                }
            }

            foreach (var attribute in variant.Attributes)
            {
                if (string.IsNullOrWhiteSpace(attribute.Value))
                    throw new Exception($"El campo {attribute.Field} del detalle esta vacio.");
            }
        }
    }
}
