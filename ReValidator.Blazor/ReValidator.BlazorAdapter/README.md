# ReValidator.BlazorAdapter

A Blazor adapter for the [ReValidator](https://www.nuget.org/packages/ReValidator) validation library. This package enables seamless integration of ReValidator with Blazor's `EditForm` and validation system, providing model and field-level validation for your Blazor applications.

## Features
- Integrates ReValidator with Blazor's `EditForm`.
- Supports model and field-level validation.
- Works with dependency injection for validator resolution.
- Merges server-side RFC 7807 validation errors into the form via `ServerValidationInterop`.
- Compatible with .NET 10.

## Installation

1. Add the NuGet package to your Blazor project:
   ```shell
   dotnet add package ReValidator.BlazorAdapter
   ```
2. Add your validators (implementing `IValidator<TModel>`) to your DI container, e.g.:
   ```csharp
   builder.Services.AddScoped<IValidator<ContactModel>, ContactModelValidator>();
   ```

## Usage

Place `ReValidatorComponent<TModel>` inside an `EditForm`. The component resolves `IServiceProvider` internally — no extra injection is required.

```razor
@using ReValidator.BlazorAdapter

<EditForm EditContext="editContext" OnValidSubmit="HandleValidSubmit">
    <ReValidatorComponent TModel="ContactModel" />

    <InputText @bind-Value="model.Name" />
    <ValidationMessage For="() => model.Name" />

    <button type="submit">Submit</button>
</EditForm>

@code {
    private ContactModel model = new();
    private EditContext editContext = null!;

    protected override void OnInitialized()
    {
        editContext = new EditContext(model);
    }
}
```

## Server-Side Validation

`ServerValidationInterop.HandleResponse` merges errors from an HTTP **422 Unprocessable Entity** response (RFC 7807 `ValidationProblem`) into the form's `ValidationMessageStore`, so server errors appear alongside client errors.

```razor
@code {
    private ValidationMessageStore serverMessageStore = null!;

    protected override void OnInitialized()
    {
        editContext = new EditContext(model);
        serverMessageStore = new ValidationMessageStore(editContext);
    }

    private async Task HandleValidSubmit()
    {
        serverMessageStore.Clear();
        editContext.NotifyValidationStateChanged();

        var response = await Http.PostAsJsonAsync("/api/contact", model);
        var body = await response.Content.ReadAsStringAsync();

        if (ServerValidationInterop.HandleResponse(response, body, editContext, serverMessageStore))
        {
            // success
        }
    }
}
```

## Requirements
- .NET 10.0 or later
- Blazor Server (Interactive Server rendering)
- [ReValidator](https://www.nuget.org/packages/ReValidator)

## License
MIT
