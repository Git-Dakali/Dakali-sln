using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel
{
    public class VariantRequest : RequestGuid
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal SalePrice { get; set; }
        public bool Active { get; set; }
        public IList<ColorRequest> ColorsHex { get; set; }
        public IList<AttributeGroupRequest> AttributeGroups { get; set; }
    }
}
