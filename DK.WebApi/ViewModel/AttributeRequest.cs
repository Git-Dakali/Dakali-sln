using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel
{
    public class AttributeRequest: RequestGuid
    {
        public string Field { get; set; }
        public string Value { get; set; }
    }
}
