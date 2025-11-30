using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel
{
    public class PropertyGroupRequest : RequestGuid
    {
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public IEnumerable<PropertyRequest> Properties { get; set; }
    }
}
