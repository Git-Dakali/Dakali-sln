namespace DK.Domain.Meta
{
    public class WebHookEvent
    {
        public long Id { get; set; }
        public EventType EventType { get; set; }
        public string JSon { get; set; }
        public bool IsProcessed { get; set; }
        public string Error { get; set; }
    }
}
