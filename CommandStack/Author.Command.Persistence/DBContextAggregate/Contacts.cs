using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.DBContextAggregate
{
    public partial class Contacts
    {
        public Contacts()
        {
            ArticleRelatedContacts = new HashSet<ArticleRelatedContacts>();
            ContactContents = new HashSet<ContactContents>();
        }

        public int ContactId { get; set; }
        public int? ImageId { get; set; }
        public string OfficePhone { get; set; }
        public string OfficeFaxNumber { get; set; }
        public string MobilePhoneNumber { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string FaceBookUrl { get; set; }
        public string TwitterUrl { get; set; }
        public string LinkedInUrl { get; set; }
        public string GooglePlusUrl { get; set; }
        public string WeChatUrl { get; set; }
        public string WeboUrl { get; set; }
        public string Xingurl { get; set; }
        public string Vkurl { get; set; }
        public int? CountryId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string EmpGuid { get; set; }
        public bool OfficePhoneEdited { get; set; }
        public bool MobilePhoneNumberEdited { get; set; }
        public bool ImageIdEdited { get; set; }

        public virtual Countries Country { get; set; }
        public virtual Images Image { get; set; }
        public virtual ICollection<ArticleRelatedContacts> ArticleRelatedContacts { get; set; }
        public virtual ICollection<ContactContents> ContactContents { get; set; }
    }
}
