using Lumle.Api.Infrastructures.Validators.AccountVM;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Lumle.Api.ViewModels.Account
{
    public class SignupVM : IValidatableObject
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int Gender { get; set; } //1 = Male, 2 = Female, 3 = Other

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new SignupVMValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }
}
