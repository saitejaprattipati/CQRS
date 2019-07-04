using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.DBContextAggregate
{
    public partial class Pdicontacts
    {
        public int PdiContactId { get; set; }
        public string EmpGuid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NativeName { get; set; }
        public string Title { get; set; }
        public string Role { get; set; }
        public string EmployeeLevel { get; set; }
        public string OfficePhone { get; set; }
        public string MobilePhoneNumber { get; set; }
        public string Email { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostCode { get; set; }
        public string Country { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
