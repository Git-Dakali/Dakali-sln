namespace DK.WebApi.ViewModel.Base
{
    public class RequestGuid : Request
    {
        public Guid Guid { get; set; }
        public string SearchString { get; set; }
    }
}
