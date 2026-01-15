using Microsoft.Extensions.DependencyInjection;

namespace DK.Validator
{
    public class DependencyInjectionValidator
    {
        public static void Configure(IServiceCollection service)
        {
            service.AddScoped<CategoryValidator>();
            service.AddScoped<ModelValidator>();
            service.AddScoped<ProductValidator>();
            service.AddScoped<StockValidator>();
            service.AddScoped<LocationStateValidator>();
            service.AddScoped<HallwayValidator>();
            service.AddScoped<ColumnValidator>();
            service.AddScoped<LevelValidator>();
            service.AddScoped<LocationValidator>();
        }
    }
}
