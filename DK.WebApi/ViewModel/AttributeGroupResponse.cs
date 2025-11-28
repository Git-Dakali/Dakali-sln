using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel
{
    public class AttributeGroupResponse : RequestGuid
    {
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public IEnumerable<AttributeResponse> Attributes { get; set; }
    }
}
