using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel
{
    public class ProductResponse : ResponseGuid
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ModelResponse Model { get; set; }
        public IEnumerable<VariantResponse> Variants { get; set; }
    }
}
