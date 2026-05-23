using AutoMapper;
using Dakali.Domine;
using DK.Domain.GeographicLocation;
using DK.Domain.Locations;
using DK.Domain.Products;
using DK.Domain.RoadMaps;
using DK.Domain.Sales;
using DK.WebApi.ViewModel;
using DK.WebApi.ViewModel.Base;
using DK.WebApi.ViewModel.GeographicLocation.Cities;
using DK.WebApi.ViewModel.GeographicLocation.Countries;
using DK.WebApi.ViewModel.GeographicLocation.Provinces;
using DK.WebApi.ViewModel.RoadMaps;
using DK.WebApi.ViewModel.Sales;

namespace DK.WebApi
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<PropertyGroupRequest, PropertyGroup>();
            CreateMap<PropertyGroup, PropertyGroupResponse>();
            CreateMap<PropertyRequest, Property>();
            CreateMap<Property, PropertyResponse>();
            CreateMap<CategoryRequest, Category>();
            CreateMap<Category, CategoryResponse>();
            CreateMap<ProductColorRequest, ProductColor>();
            CreateMap<ProductColor, ProductColorResponse>();
            CreateMap<FieldGroupRequest, FieldGroup>();
            CreateMap<FieldGroup, FieldGroupResponse>();
            CreateMap<FieldRequest, Field>();
            CreateMap<Field, FieldResponse>();
            CreateMap<ImageRequest, Image>();
            CreateMap<Image, ImageResponse>();
            CreateMap<ModelRequest, Model>();
            CreateMap<Model, ModelResponse>();
            CreateMap<ProductRequest, Product>();
            CreateMap<Product, ProductResponse>();
            CreateMap<StoredFileRequest, StoredFile>();
            CreateMap<StoredFile, StoredFileResponse>();
            CreateMap<VariantRequest, Variant>();
            CreateMap<Variant, VariantResponse>();
            CreateMap<LocationStateRequest, LocationState>();
            CreateMap<LocationState, LocationStateResponse>();
            CreateMap<LocationRequest, Location>();
            CreateMap<Location, LocationResponse>();
            CreateMap<HallwayRequest, Hallway>();
            CreateMap<Hallway, HallwayResponse>();
            CreateMap<ColumnRequest, Column>();
            CreateMap<Column, ColumnResponse>();
            CreateMap<LevelRequest, Level>();
            CreateMap<Level, LevelResponse>();
            CreateMap<StockRequest, Stock>();
            CreateMap<Stock, StockResponse>();
            CreateMap<CountryRequest, Country>();
            CreateMap<Country, CountryResponse>();
            CreateMap<ProvinceRequest, Province>();
            CreateMap<Province, ProvinceResponse>();
            CreateMap<CityRequest, City>();
            CreateMap<City, CityResponse>();
            CreateMap<OriginSaleRequest, OriginSale>();
            CreateMap<OriginSale, OriginSaleResponse>();
            CreateMap<SaleDetailRequest, SaleDetail>();
            CreateMap<SaleDetail, SaleDetailResponse>();
            CreateMap<SaleRequest, Sale>();
            CreateMap<Sale, SaleResponse>();
            CreateMap<TaxStatusRequest, TaxStatus>();
            CreateMap<TaxStatus, TaxStatusResponse>();
            CreateMap<DriverRequest, Driver>();
            CreateMap<Driver, DriverResponse>();
            CreateMap<RoadMapRequest, RoadMap>();
            CreateMap<RoadMap, RoadMapResponse>();
            CreateMap<RoadMapSaleRequest, RoadMapSale>();
            CreateMap<RoadMapSale, RoadMapSaleResponse>();
            CreateMap<ResultPage<City>, ResultPageResponse<CityResponse>>();
            CreateMap<ResultPage<Sale>, ResultPageResponse<SaleResponse>>();
            CreateMap<ResultPage<RoadMap>, ResultPageResponse<RoadMapResponse>>();
            CreateMap<ResultPage<Product>, ResultPageResponse<ProductResponse>>();
            CreateMap<HistoricSale, HistoricSaleResponse>();
        }
    }
}
