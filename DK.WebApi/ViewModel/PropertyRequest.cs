using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel
{
    public class PropertyRequest: RequestGuid
    {
        public string Field { get; set; }
        public string Value { get; set; }
    }
}
