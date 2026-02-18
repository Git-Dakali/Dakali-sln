using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel
{
    public class ModelRequest : RequestCode
    {
        public CategoryRequest? Category { get; set; }
        public IEnumerable<FieldGroupRequest> FieldGroups { get; set; }
        public List<string> VariantNames { get; set; }
    }
}
