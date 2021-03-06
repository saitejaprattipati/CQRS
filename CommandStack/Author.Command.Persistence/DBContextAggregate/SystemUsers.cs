using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.DBContextAggregate
{
    public partial class SystemUsers
    {
        public SystemUsers()
        {
            SystemUserAssociatedCountries = new HashSet<SystemUserAssociatedCountries>();
        }

        public int SystemUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Level { get; set; }
        public string WorkPhoneNumber { get; set; }
        public string MobilePhoneNumber { get; set; }
        public string Location { get; set; }
        public string Email { get; set; }
        public int Role { get; set; }
        public TimeSpan? TimeZone { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual ICollection<SystemUserAssociatedCountries> SystemUserAssociatedCountries { get; set; }
    }
}
