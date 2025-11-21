using AutoMapper;
using DK.Domain.Products;
using DK.WebApi.ViewModel;

namespace DK.WebApi
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<CategoryRequest, Category>();
            CreateMap<Category, CategoryResponse>();
            CreateMap<FieldGroupRequest, FieldGroup>();
            CreateMap<FieldGroup, FieldGroupResponse>();
            CreateMap<FieldRequest, Field>();
            CreateMap<Field, FieldResponse>();
            CreateMap<ModelRequest, Model>();
            CreateMap<Model, ModelResponse>();
            CreateMap<SizeRequest, Size>();
            CreateMap<Size, SizeResponse>();
        }
    }
}
