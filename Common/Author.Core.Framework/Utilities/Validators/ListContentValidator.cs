using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Author.Core.Framework.Utilities.Validators
{
    public class EnsureAllListElementsArePositiveIntegersAttribute: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is List<int> list)
            {
                var result = list.Exists(i => i < 0);

                return !result ? ValidationResult.Success : new ValidationResult("Only positive numbers allowed");
            }
            else
            {
                return new ValidationResult("" + validationContext.DisplayName + " is required");
            }
        }
    }
}
