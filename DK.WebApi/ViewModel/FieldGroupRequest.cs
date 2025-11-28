using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel
{
    public class FieldGroupRequest : RequestGuid
    {
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public IEnumerable<FieldRequest> Fields { get; set; }
    }
}
