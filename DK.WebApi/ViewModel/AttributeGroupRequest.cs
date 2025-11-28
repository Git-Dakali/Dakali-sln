using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel
{
    public class AttributeGroupRequest : RequestGuid
    {
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public IEnumerable<AttributeRequest> Attributes { get; set; }
    }
}
