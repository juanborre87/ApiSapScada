namespace Domain.Models
{
    public class EventPayload<T>
    {
        public string Type { get; set; }
        public string Specversion { get; set; }
        public string Source { get; set; }
        public string Id { get; set; }
        public DateTime Time { get; set; }
        public string Datacontenttype { get; set; }
        public T Data { get; set; }
    }
}
