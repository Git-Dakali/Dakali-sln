using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel
{
    public class ProductRequest : RequestGuid
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ModelRequest? Model { get; set; }
        public IEnumerable<VariantRequest> Variants { get; set; }
    }
}
