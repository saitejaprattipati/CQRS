namespace Author.Query.Domain.DBAggregate
{
    public class ResourceGroups
    {
        public string id { get; set; }
        public int ResourceGroupId { get; set; }
        public bool IsPublished { get; set; }
        public int Position { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedDate { get; set; }
        public int? ResourceGroupContentId { get; set; }
        public int LanguageId { get; set; }
        public string GroupName { get; set; }      
    }
}
