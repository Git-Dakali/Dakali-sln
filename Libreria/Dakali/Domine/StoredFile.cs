using Dakali.Domine.Base;

namespace Dakali.Domine
{
    public class StoredFile : EntityGuid
    {
        public string FileName { get; set; }
        public byte[] Content { get; set; }
        public string Module { get; set; }

        public StoredFile()
        {
            FileName = string.Empty;
            Content = new byte[0];
            Module = string.Empty;
        }

        public StoredFile(string module, string fileName, byte[] content)
        {
            FileName = fileName;
            Content = content;
            Module = module;
        }
    }
}
