using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel
{
    public class ImageRequest : RequestGuid
    {
        public StoredFileRequest File { get; set; }
        public bool IsPrimary { get; set; }
        public int SortOrder { get; set; }
    }
}
