using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.DBContextAggregate
{
    public partial class ResourceGroups
    {
        public ResourceGroups()
        {
            Articles = new HashSet<Articles>();
            ResourceGroupContents = new HashSet<ResourceGroupContents>();
        }

        public int ResourceGroupId { get; set; }
        public bool IsPublished { get; set; }
        public int Position { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual ICollection<Articles> Articles { get; set; }
        public virtual ICollection<ResourceGroupContents> ResourceGroupContents { get; set; }
    }
}
