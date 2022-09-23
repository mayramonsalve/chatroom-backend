using FluentValidation;
using Jobsity.Auth.DTOs;

namespace Jobsity.Auth.Validators
{
    public class AccountValidator
    {
        public class RegisterValidator : AbstractValidator<RegisterDTO>
        {
            public RegisterValidator()
            {
                RuleFor(register => register)
                    .NotNull().WithMessage("{PropertyName} must not be null.");

                RuleFor(register => register.Email)
                    .NotEmpty().WithMessage("{PropertyName} must not be empty.")
                    .EmailAddress().WithMessage("{PropertyName} not valid.")
                    .Length(5, 100).WithMessage("{PropertyName} must have at least 5 characters and at most 100 characters.");

                RuleFor(signin => signin.Username)
                    .NotEmpty().WithMessage("{PropertyName} must not be empty.")
                    .Length(5, 100).WithMessage("{PropertyName} must have at least 5 characters and at most 100 characters.");

                RuleFor(account => account.Password)
                    .NotEmpty().WithMessage("{PropertyName} must not be empty.");

                RuleFor(account => account.ConfirmPassword)
                    .NotEmpty().WithMessage("{PropertyName} must not be empty.");
            }
        }

        public class PasswordValidator : AbstractValidator<PasswordDTO>
        {
            public PasswordValidator()
            {
                RuleFor(password => password)
                    .NotNull().WithMessage("{PropertyName} must not be null.");

                RuleFor(password => password.Password)
                .NotEmpty().WithMessage("{PropertyName} must not be empty.")
                .Matches("[A-Z]").WithMessage("{PropertyName} must contain at least one capital letter.")
                .Matches("[a-z]").WithMessage("{PropertyName} must contain at least one lowercase letter.")
                .Matches(@"\d").WithMessage("{PropertyName} must contain at least one numeric digit.")
                .Matches(@"[][""!@$%^&*(){}:;<>,.?/+_=|'~\\-]").WithMessage("{PropertyName} must contain at least one special character.")
                .Length(7, 100).WithMessage("{PropertyName} must have at least 7 characters and at most 100 characters.");

                RuleFor(password => password.ConfirmPassword)
                    .NotEmpty().WithMessage("{PropertyName} must not be empty.")
                    .Matches(x => x.Password).WithMessage("Password and Confirm Password must match.");

            }
        }

        public class SignInValidator : AbstractValidator<SignInDTO>
        {
            public SignInValidator()
            {
                RuleFor(signin => signin)
                    .NotNull().WithMessage("{PropertyName} must not be null.");

                RuleFor(signin => signin.Username)
                    .NotEmpty().WithMessage("{PropertyName} must not be empty.")
                    .Length(5, 100).WithMessage("{PropertyName} must have at least 5 characters and at most 100 characters.");

                RuleFor(signin => signin.Password)
                    .NotEmpty().WithMessage("{PropertyName} must not be empty.");
            }
        }
    }
}
