using ReValidator.Shared.Models;

namespace ReValidator.Shared.Validators;

public class RegisterModelValidator : ReValidator.Validator<RegisterModel>
{
    public RegisterModelValidator()
    {
        SetRule(new ValidationDefinition<RegisterModel>
        {
            PropertyName = nameof(RegisterModel.Username),
            RuleName = "UsernameRequired",
            Rule = m => !string.IsNullOrWhiteSpace(m.Username),
            ErrorMessage = "Username is required."
        });

        SetRule(new ValidationDefinition<RegisterModel>
        {
            PropertyName = nameof(RegisterModel.Username),
            RuleName = "UsernameLength",
            Rule = m => string.IsNullOrWhiteSpace(m.Username) || (m.Username.Length >= 3 && m.Username.Length <= 20),
            ErrorMessage = "Username must be between 3 and 20 characters."
        });

        SetRule(new ValidationDefinition<RegisterModel>
        {
            PropertyName = nameof(RegisterModel.Email),
            RuleName = "EmailRequired",
            Rule = m => !string.IsNullOrWhiteSpace(m.Email),
            ErrorMessage = "Email is required."
        });

        SetRule(new ValidationDefinition<RegisterModel>
        {
            PropertyName = nameof(RegisterModel.Email),
            RuleName = "EmailFormat",
            Rule = m => string.IsNullOrWhiteSpace(m.Email) || m.Email.Contains('@'),
            ErrorMessage = "Email must be a valid email address."
        });

        SetRule(new ValidationDefinition<RegisterModel>
        {
            PropertyName = nameof(RegisterModel.Password),
            RuleName = "PasswordRequired",
            Rule = m => !string.IsNullOrWhiteSpace(m.Password),
            ErrorMessage = "Password is required."
        });

        SetRule(new ValidationDefinition<RegisterModel>
        {
            PropertyName = nameof(RegisterModel.Password),
            RuleName = "PasswordMinLength",
            Rule = m => string.IsNullOrWhiteSpace(m.Password) || m.Password.Length >= 8,
            ErrorMessage = "Password must be at least 8 characters."
        });

        SetRule(new ValidationDefinition<RegisterModel>
        {
            PropertyName = nameof(RegisterModel.ConfirmPassword),
            RuleName = "ConfirmPasswordMatch",
            Rule = m => m.Password == m.ConfirmPassword,
            ErrorMessage = "Passwords do not match."
        });

        SetRule(new ValidationDefinition<RegisterModel>
        {
            PropertyName = nameof(RegisterModel.Age),
            RuleName = "AgeRange",
            Rule = m => m.Age >= 18 && m.Age <= 120,
            ErrorMessage = "Age must be between 18 and 120."
        });

        SetRule(new ValidationDefinition<RegisterModel>
        {
            PropertyName = nameof(RegisterModel.AcceptTerms),
            RuleName = "TermsAccepted",
            Rule = m => m.AcceptTerms,
            ErrorMessage = "You must accept the terms and conditions."
        });
    }
}
