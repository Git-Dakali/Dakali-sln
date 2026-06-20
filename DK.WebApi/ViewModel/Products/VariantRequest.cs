using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel.Products
{
    public class VariantRequest : RequestGuid
    {
        public string Name { get; set; }
        public int SortOrder { get; set; }
    }
}
