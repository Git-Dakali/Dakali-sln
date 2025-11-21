namespace DK.WebApi.ViewModel
{
    public class ModelRequest
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public CategoryRequest Category { get; set; }
        public IEnumerable<FieldGroupRequest> FieldGroups { get; set; }
        public List<SizeRequest> Sizes { get; set; }
    }
}
