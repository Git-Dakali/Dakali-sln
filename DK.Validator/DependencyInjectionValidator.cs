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
            service.AddScoped<StockStateValidator>();
        }
    }
}
