using Author.Core.Framework;
using System.ComponentModel.DataAnnotations;

namespace Author.Command.Domain.Models
{
    public class LanguageContent
    {
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public int LanguageId { get; set; }
        [Required]
        [RegularExpression(Constants.GeneralStringRegularExpression)]
        public string TeaserText { get; set; }
        [Required]
        [RegularExpression(Constants.GeneralStringRegularExpression)]
        public string Body { get; set; }
        [Required]
        [RegularExpression(Constants.GeneralStringRegularExpression)]
        public string Title { get; set; }
    }
}
