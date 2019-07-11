using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Author.Core.Framework.Utilities.Validators
{
    public class EmailValidator: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string email = value.ToString();

                if (Regex.IsMatch(email, @"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", RegexOptions.IgnoreCase))
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("Please Enter a Valid Email.");
                }
            }
            else
            {
                return new ValidationResult("" + validationContext.DisplayName + " is required");
            }
        }
    }
}
