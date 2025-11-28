using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel
{
    public class ColorResponse: ResponseGuid
    {
        public string Name { get; set; }
        public string Hex { get; set; }
        public int SortOrder { get; set; }
        public IList<ImageResponse> Images { get; set; }
    }
}
