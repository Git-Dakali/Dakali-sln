using DK.Repositories.Products;
using Microsoft.Extensions.DependencyInjection;

namespace DK.Repositories
{
    public class DependencyInjectionRepository
    {
        public static void Configure(IServiceCollection service)
        {
            service.AddScoped<PropertyGroupRepository>();
            service.AddScoped<PropertyRepository>();
            service.AddScoped<CategoryRepository>();
            service.AddScoped<ColorRepository>();
            service.AddScoped<FieldGroupRepository>();
            service.AddScoped<FieldRepository>();
            service.AddScoped<ColorImageRepository>();
            service.AddScoped<ModelRepository>();
            service.AddScoped<ProductRepository>();
            service.AddScoped<Model_VariantNameRepository>();
            service.AddScoped<StoredFileRepository>();
            service.AddScoped<VariantRepository>();
        }
    }
}
