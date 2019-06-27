using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.Author.Command.API.ArticleAggregate
{
    public partial class Skills
    {
        public int SkillId { get; set; }
        public string Speciality { get; set; }
        public int? ContactContentId { get; set; }

        public ContactContents ContactContent { get; set; }
    }
}
