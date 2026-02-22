# ReValidator.BlazorAdapter

A Blazor adapter for the [ReValidator](https://www.nuget.org/packages/ReValidator) validation library. This package enables seamless integration of ReValidator with Blazor's `EditForm` and validation system, providing model and field-level validation for your Blazor applications.

## Features
- Integrates ReValidator with Blazor's `EditForm`.
- Supports model and field-level validation.
- Works with dependency injection for validator resolution.
- Compatible with .NET 10 and C# 14.0.

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

1. Wrap your `EditForm` with the `ReValidatorComponent<TModel>` in your Blazor page/component:
   ```razor
   @using ReValidator.BlazorAdapter
   @inject IServiceProvider ServiceProvider

   <EditForm Model="@model" OnValidSubmit="HandleValidSubmit">
       <ReValidatorComponent TModel="ContactModel" />
       <!-- form fields -->
   </EditForm>
   ```
2. Ensure your validator is registered for the model type.

## Example
```csharp
// In Program.cs or Startup.cs
builder.Services.AddScoped<IValidator<ContactModel>, ContactModelValidator>();
```

```razor
<EditForm Model="@contact" OnValidSubmit="OnSubmit">
    <ReValidatorComponent TModel="ContactModel" />
    <InputText @bind-Value="contact.Name" />
    <ValidationMessage For="@(() => contact.Name)" />
    <button type="submit">Submit</button>
</EditForm>
```

## Requirements
- .NET 10.0 or later
- Blazor (Server or WebAssembly)
- [ReValidator](https://www.nuget.org/packages/ReValidator)

## License
MIT
