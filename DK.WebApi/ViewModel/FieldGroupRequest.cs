namespace DK.WebApi.ViewModel
{
    public class FieldGroupRequest
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public IEnumerable<FieldRequest> Fields { get; set; }
    }
}
