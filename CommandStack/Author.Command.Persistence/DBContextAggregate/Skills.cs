using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.DBContextAggregate
{
    public partial class Skills
    {
        public int SkillId { get; set; }
        public string Speciality { get; set; }
        public int? ContactContentId { get; set; }

        public virtual ContactContents ContactContent { get; set; }
    }
}
