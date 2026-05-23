using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel
{
    public class ProductColorResponse: ResponseGuid
    {
        public string Name { get; set; }
        public string Hex { get; set; }
        public string Sku { get; set; }
        public int SortOrder { get; set; }
        public IList<ImageResponse> Images { get; set; }
    }
}
