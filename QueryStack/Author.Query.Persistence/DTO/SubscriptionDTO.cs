namespace Author.Query.Persistence.DTO
{
    public class SubscriptionDTO
    {
        public int TagUUID { get; set; }
        public int CountryUUID { get; set; }
        public bool IsSubscribed { get; set; }
    }
}
