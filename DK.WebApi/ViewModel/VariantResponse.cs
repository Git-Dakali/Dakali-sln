using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel
{
    public class VariantResponse : ResponseGuid
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal SalePrice { get; set; }
        public bool Active { get; set; }
        public IList<ColorResponse> ColorsHex { get; set; }
        public IList<AttributeGroupResponse> AttributeGroups { get; set; }
    }
}
