using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel
{
    public class StoredFileResponse : ResponseGuid
    {
        public string FileName { get; set; }
        public string Module { get; set; }
        public string? ContentBase64 { get; set; }
    }
}
