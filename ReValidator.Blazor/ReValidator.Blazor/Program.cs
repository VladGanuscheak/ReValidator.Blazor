using ReValidator;
using ReValidator.Blazor.Components;
using System.Text.Json;
using ReValidator.Contracts;
using ReValidator.Validation.MinimalApi;
using Blazor.Shared.Models;
using ReValidator.SetUp;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<ReValidatorOptions>();

builder.Services.AddReValidator();

// HttpClient for calling the ReValidator API
builder.Services.AddHttpClient("ReValidatorApi", client =>
{
    client.BaseAddress = new Uri("http://localhost:5129");
});

var rulesDir = Path.Combine(builder.Environment.ContentRootPath, "ReconfigurationRules");

var app = builder.Build();

// Load and apply dynamic validation rules from JSON files
if (Directory.Exists(rulesDir))
{
    foreach (var file in Directory.GetFiles(rulesDir, "*.json"))
    {
        var json = File.ReadAllText(file);
        var configs = JsonSerializer.Deserialize<List<DynamicReconfiguration>>(json);
        if (configs != null)
        {
            foreach (var config in configs)
            {
                app.Services.ApplyReconfiguration(config);
            }
        }
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Validation API endpoints (mirrored from ReValidator.Api)
//app.MapPost("/api/contact", (ContactModel model, IValidator<ContactModel> validator) =>
//{
//    var result = validator.Validate(model);
//    return result.IsValid
//        ? Results.Ok(new { message = "Contact form submitted successfully." })
//        : Results.ValidationProblem(
//            result.Errors.ToDictionary(e => e.PropertyName, e => e.ErrorMessages));
//});

//app.MapPost("/api/register", (RegisterModel model, IValidator<RegisterModel> validator) =>
//{
//    var result = validator.Validate(model);
//    return result.IsValid
//        ? Results.Ok(new { message = $"User '{model.Username}' registered successfully." })
//        : Results.ValidationProblem(
//            result.Errors.ToDictionary(e => e.PropertyName, e => e.ErrorMessages));
//});

app.MapPost("/api/contact", (ContactModel model, IValidator<ContactModel> validator) =>
{
    var result = validator.Validate(model);

    Results.Ok(new { message = "Contact form submitted successfully." });
})
.WithName("SubmitContact")
.AddReValidator<ContactModel>();


app.MapPost("/api/register", (RegisterModel model, IValidator<RegisterModel> validator) =>
{
    var result = validator.Validate(model);
    return result.IsValid
        ? Results.Ok(new { message = $"User '{model.Username}' registered successfully." })
        : Results.ValidationProblem(
            result.Errors.ToDictionary(e => e.PropertyName, e => e.ErrorMessages));
})
.WithName("RegisterUser")
.AddReValidator<RegisterModel>();

// Rule management endpoints
app.MapGet("/api/rules", () =>
{
    var rules = new List<DynamicReconfiguration>();
    if (Directory.Exists(rulesDir))
    {
        foreach (var file in Directory.GetFiles(rulesDir, "*.json"))
        {
            var json = File.ReadAllText(file);
            var configs = JsonSerializer.Deserialize<List<DynamicReconfiguration>>(json);
            if (configs != null)
                rules.AddRange(configs);
        }
    }
    return rules;
});

app.MapPost("/api/rules", async (DynamicReconfiguration config) =>
{
    if (!Directory.Exists(rulesDir))
        Directory.CreateDirectory(rulesDir);

    var modelName = config.FullPathToModel?.Split('.').LastOrDefault() ?? "Default";
    var file = Path.Combine(rulesDir, $"{modelName}.json");
    List<DynamicReconfiguration> configs = new();
    if (File.Exists(file))
    {
        var json = await File.ReadAllTextAsync(file);
        var existing = JsonSerializer.Deserialize<List<DynamicReconfiguration>>(json);
        if (existing != null)
            configs.AddRange(existing);
    }
    configs.Add(config);
    await File.WriteAllTextAsync(file, JsonSerializer.Serialize(configs, new JsonSerializerOptions { WriteIndented = true }));
    app.Services.ApplyReconfiguration(config);
    return Results.Ok();
});

app.Run();
