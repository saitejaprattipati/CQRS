using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.DBContextAggregate
{
    public partial class Pdiimages
    {
        public int PdiimageId { get; set; }
        public string Name { get; set; }
        public int? ImageType { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public string EmailAddress { get; set; }
        public string EmpGuid { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
