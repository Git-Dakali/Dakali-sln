namespace DK.WebApi.ViewModel.Base
{
    public class ResultPageResponse<T> where T : Response
    {
        public IEnumerable<T> Values { get; set; }
        public long Count { get; set; }
    }
}
