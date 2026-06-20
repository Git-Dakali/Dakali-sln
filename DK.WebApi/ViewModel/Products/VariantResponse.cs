using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel.Products
{
    public class VariantResponse : ResponseGuid
    {
        public string Name { get; set; }
        public int SortOrder { get; set; }
    }
}
