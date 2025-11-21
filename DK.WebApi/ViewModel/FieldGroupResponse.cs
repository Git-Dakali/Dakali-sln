namespace DK.WebApi.ViewModel
{
    public class FieldGroupResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public IEnumerable<FieldResponse> Fields { get; set; }
    }
}
