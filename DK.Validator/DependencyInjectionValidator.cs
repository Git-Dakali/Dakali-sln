using DK.Validator.GeographicLocation;
using DK.Validator.Products;
using DK.Validator.ReturnOrders;
using DK.Validator.RoadMaps;
using DK.Validator.Sales;
using Microsoft.Extensions.DependencyInjection;

namespace DK.Validator
{
    public class DependencyInjectionValidator
    {
        public static void Configure(IServiceCollection service)
        {
            service.AddScoped<CategoryValidator>();
            service.AddScoped<FieldValidator>();
            service.AddScoped<ProductSkuValidator>();
            service.AddScoped<CategoryValidator>();
            service.AddScoped<ProductValidator>();
            service.AddScoped<StockValidator>();
            service.AddScoped<LocationStateValidator>();
            service.AddScoped<HallwayValidator>();
            service.AddScoped<ColumnValidator>();
            service.AddScoped<LevelValidator>();
            service.AddScoped<LocationValidator>();
            service.AddScoped<ProductColorValidator>();
            service.AddScoped<VariantValidator>();
            service.AddScoped<CountryValidator>();
            service.AddScoped<ProvinceValidator>();
            service.AddScoped<CityValidator>();
            service.AddScoped<OriginSaleValidator>();
            service.AddScoped<SaleValidator>();
            service.AddScoped<SaleDetailValidator>();
            service.AddScoped<TaxStatusValidator>();
            service.AddScoped<DriverValidator>();
            service.AddScoped<RoadMapValidator>();
            service.AddScoped<RoadMapSaleValidator>();
            service.AddScoped<LogisticsProviderValidator>();
            service.AddScoped<ReturnOrderValidator>();
        }
    }
}
