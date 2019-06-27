using System;
using System.Collections.Generic;

namespace Author.Command.Persistence.Author.Command.API.ArticleAggregate
{
    public partial class DisclaimerContents
    {
        public int DisclaimerContentId { get; set; }
        public int? LanguageId { get; set; }
        public string ProviderName { get; set; }
        public string ProviderTerms { get; set; }
        public int? DisclaimerId { get; set; }

        public Disclaimers Disclaimer { get; set; }
        public Languages Language { get; set; }
    }
}
