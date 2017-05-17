using FluentValidation;
using Lumle.Api.ViewModels.Account;

namespace Lumle.Api.Infrastructures.Validators.AccountVM
{
    public class SignupVMValidator : AbstractValidator<SignupVM>
    {

        public SignupVMValidator()
        {
            RuleFor(model => model.Email)
                .EmailAddress().WithMessage("Please enter valid email address.");

            RuleFor(model => model.FirstName)
                .NotEmpty().WithMessage("Firstname cannot be empty");

            RuleFor(model => model.LastName)
                .NotEmpty().WithMessage("Lastname cannot be empty");

            RuleFor(model => model.Gender)
                .NotEmpty().WithMessage("Gender cannot be empty.");

            RuleFor(model => model.Password)
                .Length(6, 20).WithMessage("Password length must be of 6-20 characters.");


        }


    }
}
