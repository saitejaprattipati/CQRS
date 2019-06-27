using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.Author.Command.API.ArticleAggregate
{
    public partial class ContactContents
    {
        public ContactContents()
        {
            Skills = new HashSet<Skills>();
        }

        public int ContactContentId { get; set; }
        public int? LanguageId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NativeName { get; set; }
        public string Title { get; set; }
        public string Role { get; set; }
        public string EmployeeLevel { get; set; }
        public string Introduction { get; set; }
        public string Organization { get; set; }
        public string OrganizationUnitName { get; set; }
        public int? AddressId { get; set; }
        public int? ContactId { get; set; }
        public bool FirstNameEdited { get; set; }
        public bool LastNameEdited { get; set; }
        public bool TitleEdited { get; set; }
        public bool RoleEdited { get; set; }
        public bool EmployeeLevelEdited { get; set; }

        public Addresses Address { get; set; }
        public Contacts Contact { get; set; }
        public Languages Language { get; set; }
        public ICollection<Skills> Skills { get; set; }
    }
}
