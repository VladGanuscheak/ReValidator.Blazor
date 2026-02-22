using ReValidator;
using ReValidator.Blazor.Components;
using ReValidator.Shared.Models;
using ReValidator.Shared.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<IValidator<ContactModel>, ContactModelValidator>();
builder.Services.AddScoped<IValidator<RegisterModel>, RegisterModelValidator>();

// HttpClient for calling the ReValidator API
builder.Services.AddHttpClient("ReValidatorApi", client =>
{
    client.BaseAddress = new Uri("http://localhost:5200");
});

var app = builder.Build();

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

app.Run();
