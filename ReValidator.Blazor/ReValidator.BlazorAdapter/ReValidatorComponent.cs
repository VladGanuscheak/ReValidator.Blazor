using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection; 

namespace ReValidator.BlazorAdapter;

public class ReValidatorComponent<TModel> : ComponentBase
{
    private ValidationMessageStore? _messageStore;

    [CascadingParameter]
    private EditContext? CurrentEditContext { get; set; }

    [Inject]
    private IServiceProvider ServiceProvider { get; set; } = default!;

    protected override void OnInitialized()
    {
        if (CurrentEditContext is null)
        {
            throw new InvalidOperationException(
                $"{nameof(ReValidatorComponent<>)} requires a cascading parameter of type {nameof(EditContext)}. " +
                $"For example, you can use {nameof(ReValidatorComponent<>)} inside an EditForm.");
        }

        _messageStore = new ValidationMessageStore(CurrentEditContext);

        CurrentEditContext.OnValidationRequested += OnValidationRequested;
        CurrentEditContext.OnFieldChanged += OnFieldChanged;
    }

    private void OnValidationRequested(object? sender, ValidationRequestedEventArgs e)
    {
        if (CurrentEditContext is null) return;

        _messageStore?.Clear();
        ValidateModel();
        CurrentEditContext.NotifyValidationStateChanged();
    }

    private void OnFieldChanged(object? sender, FieldChangedEventArgs e)
    {
        if (CurrentEditContext is null) return;

        _messageStore?.Clear(e.FieldIdentifier);
        ValidateField(e.FieldIdentifier);
        CurrentEditContext.NotifyValidationStateChanged();
    }

    private void ValidateModel()
    {
        var validator = GetValidator();
        if (validator is null || CurrentEditContext is null) return;

        var model = (TModel)CurrentEditContext.Model;
        var result = validator.Validate(model);

        foreach (var error in result.Errors)
        {
            var fieldIdentifier = new FieldIdentifier(CurrentEditContext.Model, error.PropertyName);
            _messageStore?.Add(fieldIdentifier, error.ErrorMessages);
        }
    }

    private void ValidateField(FieldIdentifier fieldIdentifier)
    {
        var validator = GetValidator();
        if (validator is null || CurrentEditContext is null) return;

        var model = (TModel)CurrentEditContext.Model;
        var result = validator.Validate(model);

        foreach (var error in result.Errors.Where(e =>
            string.Equals(e.PropertyName, fieldIdentifier.FieldName, StringComparison.OrdinalIgnoreCase)))
        {
            _messageStore?.Add(fieldIdentifier, error.ErrorMessages);
        }
    }

    private IValidator<TModel>? GetValidator()
    {
        return ServiceProvider.GetService<IValidator<TModel>>();
    }
}
