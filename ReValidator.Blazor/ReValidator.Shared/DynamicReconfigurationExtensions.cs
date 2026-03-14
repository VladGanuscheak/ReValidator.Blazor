using Microsoft.Extensions.DependencyInjection;
using ReValidator.Contracts;

namespace ReValidator.Shared;

public static class DynamicReconfigurationExtensions
{
    public static IServiceProvider ApplyReconfiguration(
        this IServiceProvider serviceProvider,
        DynamicReconfiguration config)
    {
        if (string.IsNullOrWhiteSpace(config.FullPathToModel))
            throw new ArgumentNullException(nameof(config.FullPathToModel));

        if (string.IsNullOrWhiteSpace(config.Expression))
            throw new ArgumentNullException(nameof(config.Expression));

        var type = AppDomain.CurrentDomain.GetAssemblies()
            .Select(a => a.GetType(config.FullPathToModel))
            .FirstOrDefault(t => t != null)
            ?? throw new InvalidOperationException("Type not found");

        var options = serviceProvider.GetRequiredService<ReValidatorOptions>();

        var validationExpressionType = typeof(ValidationExpression<>).MakeGenericType(type);

        var validationExpression = Activator.CreateInstance(
            validationExpressionType,
            config.Expression!,
            config.PropertyName ?? "Model",
            config.RuleName ?? "DynamicExpression",
            config.ErrorMessage ?? $"Validation failed: {config.Expression}",
            options
        );

        var validatorType = typeof(IValidator<>).MakeGenericType(type);
        var validator = serviceProvider.GetRequiredService(validatorType);

        var setRuleMethod = validatorType.GetMethod(
            "SetRule",
            new[] { validationExpressionType });

        setRuleMethod!.Invoke(validator, new[] { validationExpression });

        return serviceProvider;
    }
}
