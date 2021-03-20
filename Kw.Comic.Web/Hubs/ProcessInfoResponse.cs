namespace KwC.Hubs
{
    public class ProcessInfoResponse
    {
        public NotifyTypes Type { get; set; }
        public string Name { get; set; }
        public string ComicUrl { get; set; }
        public ProcessItemSnapshot Chapter { get; set; }
        public ProcessItemSnapshot Page { get; set; }
    }
}
