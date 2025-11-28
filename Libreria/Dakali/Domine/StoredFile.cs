using Dakali.Domine.Base;

namespace Dakali.Domine
{
    public class StoredFile : EntityGuid
    {
        public string FileName { get; set; }
        public string ContentBase64 { get; set; }
        public string Module { get; set; }

        public StoredFile()
        {
            FileName = string.Empty;
            ContentBase64 = string.Empty;
            Module = string.Empty;
        }

        public StoredFile(string module, string fileName, string contentBase64)
        {
            FileName = fileName;
            ContentBase64 = contentBase64;
            Module = module;
        }
    }
}
