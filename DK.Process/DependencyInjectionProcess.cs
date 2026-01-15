using DK.Process.Locations;
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
            service.AddScoped<StockProcess>();
            service.AddScoped<HallwayProcess>();
            service.AddScoped<ColumnProcess>();
            service.AddScoped<LevelProcess>();
            service.AddScoped<LocationStateProcess>();
            service.AddScoped<LocationProcess>();
        }
    }
}
