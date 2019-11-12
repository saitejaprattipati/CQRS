namespace Author.Query.Persistence.DTO
{
    public class LanguageDTO
    {
        public string id { get; set; }
        public int LanguageId { get; set; }
        public string Name { get; set; }
        public string NameinEnglish { get; set; }
        public string LocalisationIdentifier { get; set; }
        public string Locale { get; set; }
        public bool RightToLeft { get; set; }
    }
}
