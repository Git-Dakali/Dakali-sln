using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel
{
    public class ImageResponse : ResponseGuid
    {
        public StoredFileResponse? File { get; set; }
        public bool IsPrimary { get; set; }
        public int SortOrder { get; set; }
    }
}
