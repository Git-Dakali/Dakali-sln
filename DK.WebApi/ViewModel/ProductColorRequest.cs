using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel
{
    public class ProductColorRequest: RequestGuid
    {
        public string Name { get; set; }
        public string Hex { get; set; }
        public int SortOrder { get; set; }
        public IList<ImageRequest> Images { get; set; }
    }
}
