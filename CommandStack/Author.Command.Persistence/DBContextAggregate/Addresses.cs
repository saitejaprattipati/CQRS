using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.DBContextAggregate
{
    public partial class Addresses
    {
        public Addresses()
        {
            AddressContents = new HashSet<AddressContents>();
            ContactContents = new HashSet<ContactContents>();
        }

        public int AddressId { get; set; }
        public string PostCode { get; set; }
        public bool PostCodeEdited { get; set; }

        public virtual ICollection<AddressContents> AddressContents { get; set; }
        public virtual ICollection<ContactContents> ContactContents { get; set; }
    }
}
