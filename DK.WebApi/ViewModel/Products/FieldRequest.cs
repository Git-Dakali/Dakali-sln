using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel.Products
{
    public class FieldRequest : RequestGuid
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public int SortOrder { get; set; }
    }
}
