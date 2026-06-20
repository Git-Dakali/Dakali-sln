using DK.Repositories.GeographicLocation;
using DK.Repositories.Locations;
using DK.Repositories.Products;
using DK.Repositories.ReturnOrders;
using DK.Repositories.RoadMaps;
using DK.Repositories.Sales;
using Microsoft.Extensions.DependencyInjection;

namespace DK.Repositories
{
    public class DependencyInjectionRepository
    {
        public static void Configure(IServiceCollection service)
        {
            service.AddScoped<CategoryRepository>();
            service.AddScoped<ProductColorRepository>();
            service.AddScoped<FieldRepository>();
            service.AddScoped<ProductColorImageRepository>();
            service.AddScoped<ProductRepository>();
            service.AddScoped<StoredFileRepository>();
            service.AddScoped<VariantRepository>();
            service.AddScoped<ProductSkuRepository>();
            service.AddScoped<LocationStateRepository>();
            service.AddScoped<StockRepository>();
            service.AddScoped<HallwayRepository>();
            service.AddScoped<ColumnRepository>();
            service.AddScoped<LevelRepository>();
            service.AddScoped<LocationRepository>();
            service.AddScoped<CountryRepository>();
            service.AddScoped<ProvinceRepository>();
            service.AddScoped<CityRepository>();
            service.AddScoped<OriginSaleRepository>();
            service.AddScoped<SaleRepository>();
            service.AddScoped<SaleDetailRepository>();
            service.AddScoped<TaxStatusRepository>();
            service.AddScoped<DriverRepository>();
            service.AddScoped<RoadMapRepository>();
            service.AddScoped<RoadMapSaleRepository>();
            service.AddScoped<HistoricSaleRepository>();
            service.AddScoped<LogisticsProviderRepository>();
            service.AddScoped<ReturnOrderRepository>();
        }
    }
}
