using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel
{
    public class FieldRequest : RequestGuid
    {
        public string Name { get; set; }
        public int SortOrder { get; set; }
    }
}
