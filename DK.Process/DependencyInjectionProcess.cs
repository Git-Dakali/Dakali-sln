using DK.Process.Product;
using Microsoft.Extensions.DependencyInjection;

namespace DK.Process
{
    public class DependencyInjectionProcess
    {
        public static void Configure(IServiceCollection service)
        {
            service.AddScoped<CategoryProcess>();
            service.AddScoped<ModelProcess>();
            service.AddScoped<ProductProcess>();
        }
    }
}
