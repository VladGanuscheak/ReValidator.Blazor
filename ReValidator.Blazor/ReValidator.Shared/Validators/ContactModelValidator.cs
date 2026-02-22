using ReValidator.Shared.Models;

namespace ReValidator.Shared.Validators;

public class ContactModelValidator : ReValidator.Validator<ContactModel>
{
    public ContactModelValidator()
    {
        SetRule(new ValidationDefinition<ContactModel>
        {
            PropertyName = nameof(ContactModel.Name),
            RuleName = "NameRequired",
            Rule = m => !string.IsNullOrWhiteSpace(m.Name),
            ErrorMessage = "Name is required."
        });

        SetRule(new ValidationDefinition<ContactModel>
        {
            PropertyName = nameof(ContactModel.Name),
            RuleName = "NameMinLength",
            Rule = m => string.IsNullOrWhiteSpace(m.Name) || m.Name.Length >= 2,
            ErrorMessage = "Name must be at least 2 characters."
        });

        SetRule(new ValidationDefinition<ContactModel>
        {
            PropertyName = nameof(ContactModel.Email),
            RuleName = "EmailRequired",
            Rule = m => !string.IsNullOrWhiteSpace(m.Email),
            ErrorMessage = "Email is required."
        });

        SetRule(new ValidationDefinition<ContactModel>
        {
            PropertyName = nameof(ContactModel.Email),
            RuleName = "EmailFormat",
            Rule = m => string.IsNullOrWhiteSpace(m.Email) || m.Email.Contains('@'),
            ErrorMessage = "Email must be a valid email address."
        });

        SetRule(new ValidationDefinition<ContactModel>
        {
            PropertyName = nameof(ContactModel.Message),
            RuleName = "MessageRequired",
            Rule = m => !string.IsNullOrWhiteSpace(m.Message),
            ErrorMessage = "Message is required."
        });

        SetRule(new ValidationDefinition<ContactModel>
        {
            PropertyName = nameof(ContactModel.Message),
            RuleName = "MessageMinLength",
            Rule = m => string.IsNullOrWhiteSpace(m.Message) || m.Message.Length >= 10,
            ErrorMessage = "Message must be at least 10 characters."
        });
    }
}
