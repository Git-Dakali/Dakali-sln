using Dakali.Domine;
using Dakali.Domine.Base;

namespace DK.Model
{
    public class ProductImage : Entity
    {
        public StoredFile File { get; set; }
        public bool IsPrimary { get; set; }
        public int SortOrder { get; set; }

        public override string ToString()
        {
            return $"{File?.FileName ?? string.Empty}";
        }
    }
}
