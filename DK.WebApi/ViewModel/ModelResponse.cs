using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel
{
    public class ModelResponse : ResponseCode
    {
        public CategoryResponse Category { get; set; }
        public IEnumerable<FieldGroupResponse> FieldGroups { get; set; }
        public List<string> VariantNames { get; set; }
    }
}
