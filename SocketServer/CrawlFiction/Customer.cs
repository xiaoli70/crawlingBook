namespace CrawlFiction
{
    public class Customer
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public int Id { get; internal set; }
    }
}