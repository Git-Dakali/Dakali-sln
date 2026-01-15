using AutoMapper;
using Dakali.Domine;
using DK.Domain.Locations;
using DK.Domain.Products;
using DK.WebApi.ViewModel;

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
            CreateMap<ColorRequest, Color>();
            CreateMap<Color, ColorResponse>();
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
        }
    }
}
