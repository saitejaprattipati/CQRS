using Author.Core.Framework;
using System.ComponentModel.DataAnnotations;

namespace Author.Command.Domain.Models
{
    public class LanguageName
    {
        //[Required]
        //[Range(0, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public string Language { get; set; }

        [RegularExpression(Constants.GeneralStringRegularExpression)]
        [Required]
        public string Name { get; set; }
    }
}
