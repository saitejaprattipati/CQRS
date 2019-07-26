using Author.Core.Framework;
using System.ComponentModel.DataAnnotations;

namespace Author.Command.Domain.Models
{
    public class DisclaimerContent
    {
        [Required]
        [RegularExpression(Constants.GeneralStringRegularExpression)]
        public string ProviderName { get; set; }

        public string ProviderTerms { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public int LanguageId { get; set; }
    }
}
