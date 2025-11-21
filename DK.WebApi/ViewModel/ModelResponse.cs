namespace DK.WebApi.ViewModel
{
    public class ModelResponse
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public CategoryResponse Category { get; set; }
        public IEnumerable<FieldGroupResponse> FieldGroups { get; set; }
        public List<SizeResponse> Sizes { get; set; }
    }
}
