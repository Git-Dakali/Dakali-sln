using DK.Repositories.GeographicLocation;
using DK.Repositories.Locations;
using DK.Repositories.Products;
using DK.Repositories.Sales;
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
            service.AddScoped<ProductColorRepository>();
            service.AddScoped<FieldGroupRepository>();
            service.AddScoped<FieldRepository>();
            service.AddScoped<ProductColorImageRepository>();
            service.AddScoped<ModelRepository>();
            service.AddScoped<ProductRepository>();
            service.AddScoped<Model_VariantNameRepository>();
            service.AddScoped<StoredFileRepository>();
            service.AddScoped<VariantRepository>();
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
        }
    }
}
