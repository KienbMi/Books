using Books.Core.Entities;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Books.Core.Validations
{
    public class IsbnValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is Book book)
            {
                return book.Validate(new ValidationContext(book)).FirstOrDefault();
            }
            
            return ValidationResult.Success;
        }
    }
}
