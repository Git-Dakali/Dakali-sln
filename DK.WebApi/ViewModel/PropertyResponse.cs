using DK.WebApi.ViewModel.Base;

namespace DK.WebApi.ViewModel
{
    public class PropertyResponse: ResponseGuid
    {
        public string Field { get; set; }
        public string Value { get; set; }
    }
}
