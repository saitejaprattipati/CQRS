using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.DBContextAggregate
{
    public partial class Disclaimers
    {
        public Disclaimers()
        {
            Articles = new HashSet<Articles>();
            DisclaimerContents = new HashSet<DisclaimerContents>();
        }

        public int DisclaimerId { get; set; }
        public string Name { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int? DefaultCountryId { get; set; }

        public virtual ICollection<Articles> Articles { get; set; }
        public virtual ICollection<DisclaimerContents> DisclaimerContents { get; set; }
    }
}
