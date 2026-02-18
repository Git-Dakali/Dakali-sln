using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel
{
    public class VariantRequest : RequestGuid
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal SalePrice { get; set; }
        public bool Active { get; set; }
        public int SortOrder { get; set; }
        public IList<ProductColorRequest> ColorsHex { get; set; }
        public IList<PropertyGroupRequest> PropertyGroups { get; set; }
    }
}
