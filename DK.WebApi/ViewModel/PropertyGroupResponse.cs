using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel
{
    public class PropertyGroupResponse : RequestGuid
    {
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public IEnumerable<PropertyResponse> Properties { get; set; }
    }
}
