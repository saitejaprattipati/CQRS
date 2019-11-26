namespace Author.Query.Persistence.DTO
{
    public class ImageDTO
    {
        public int ImageId { get; set; }
        public string Name { get; set; }
        public int ImageType { get; set; }
        public int? CountryId { get; set; }
        public string Keyword { get; set; }
        public string Source { get; set; }
        public string Copyright { get; set; }
        public string Description { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedDate { get; set; }
        public string EmpGUID { get; set; }
        public int? Edited { get; set; }
    }
}
