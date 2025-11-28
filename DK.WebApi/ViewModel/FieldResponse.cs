using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel
{
    public class FieldResponse : ResponseGuid
    {
        public string Name { get; set; }
        public int SortOrder { get; set; }
    }
}
