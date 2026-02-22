using ReValidator;
using ReValidator.Shared.Models;
using ReValidator.Shared.Validators;
using ReValidator.Validation.MinimalApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IValidator<ContactModel>, ContactModelValidator>();
builder.Services.AddScoped<IValidator<RegisterModel>, RegisterModelValidator>();

builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins("http://localhost:5129", "https://localhost:7074")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "ReValidator API");
    });
}

app.UseCors();

app.MapPost("/api/contact", (ContactModel model, IValidator<ContactModel> validator) =>
{
    var result = validator.Validate(model);

    Results.Ok(new { message = "Contact form submitted successfully." });
})
.AddReValidator<ContactModel>()
.WithName("SubmitContact")
.WithOpenApi();

app.MapPost("/api/register", (RegisterModel model, IValidator<RegisterModel> validator) =>
{
    var result = validator.Validate(model);
    return result.IsValid
        ? Results.Ok(new { message = $"User '{model.Username}' registered successfully." })
        : Results.ValidationProblem(
            result.Errors.ToDictionary(e => e.PropertyName, e => e.ErrorMessages));
})
.AddReValidator<RegisterModel>()
.WithName("RegisterUser")
.WithOpenApi();

app.Run();
