using AutoMapper;
using Dakali.Domine;
using DK.Domain.Products;
using DK.WebApi.ViewModel;

namespace DK.WebApi
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<AttributeGroupRequest, AttributeGroup>();
            CreateMap<AttributeGroup, AttributeGroupResponse>();
            CreateMap<AttributeRequest, Domain.Products.Attribute>();
            CreateMap<Domain.Products.Attribute, AttributeResponse>();
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
            CreateMap<SizeRequest, Size>();
            CreateMap<Size, SizeResponse>();
            CreateMap<StoredFileRequest, StoredFile>();
            CreateMap<StoredFile, StoredFileResponse>();
            CreateMap<VariantRequest, Variant>();
            CreateMap<Variant, VariantResponse>();
        }
    }
}
