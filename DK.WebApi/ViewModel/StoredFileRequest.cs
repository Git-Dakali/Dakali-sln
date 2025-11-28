using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel
{
    public class StoredFileRequest : RequestGuid
    {
        public string FileName { get; set; }
        public string Module { get; set; }
        public string ContentBase64 { get; set; }
    }
}
