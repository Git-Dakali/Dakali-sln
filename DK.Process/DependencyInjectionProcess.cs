using DK.Process.GeographicLocation;
using DK.Process.Locations;
using DK.Process.Product;
using DK.Process.RoadMaps;
using DK.Process.Sales;
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
            service.AddScoped<CountryProcess>();
            service.AddScoped<ProvinceProcess>();
            service.AddScoped<CityProcess>();
            service.AddScoped<OriginSaleProcess>();
            service.AddScoped<SaleProcess>();
            service.AddScoped<SaleDetailProcess>();
            service.AddScoped<TaxStatusProcess>();
            service.AddScoped<DriverProcess>();
            service.AddScoped<RoadMapProcess>();
            service.AddScoped<RoadMapSaleProcess>();
            service.AddScoped<HistoricSaleProcess>();
        }
    }
}
