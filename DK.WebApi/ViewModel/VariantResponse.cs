using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel
{
    public class VariantResponse : ResponseGuid
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal SalePrice { get; set; }
        public bool Active { get; set; }
        public int SortOrder { get; set; }
        public IList<ProductColorResponse> ColorsHex { get; set; }
        public IList<PropertyGroupResponse> PropertyGroups { get; set; }
    }
}
