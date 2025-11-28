using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel
{
    public class FieldGroupResponse : ResponseGuid
    {
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public IEnumerable<FieldResponse> Fields { get; set; }
    }
}
