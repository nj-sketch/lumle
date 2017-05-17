using JsonApiDotNetCore.Models;
using Lumle.Api.Infrastructures.Validators.AccountVM;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Lumle.Api.ViewModels.Account
{
    public class SignupVM : IValidatableObject
    {

        [Attr("first-name")]
        public string FirstName { get; set; }

        [Attr("last-name")]
        public string LastName { get; set; }

        [Attr("email")]
        public string Email { get; set; }

        [Attr("password")]
        public string Password { get; set; }

        [Attr("gender")]
        public int Gender { get; set; } //1 = Male, 2 = Female, 3 = Other, 0= unknown

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new SignupVMValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }
}
