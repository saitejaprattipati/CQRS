using System;
using System.Collections.Generic;
using System.Text;

namespace Author.Command.Domain.Models
{
   public class CommandResponse
    {
        public bool IsSuccessful { get; set; }

        public string FailureReason { get; set; }
    }
}
