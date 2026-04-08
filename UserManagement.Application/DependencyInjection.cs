using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using UserManagement.Application.Common.Behaviors;

namespace UserManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // =========================
        // ✅ MediatR
        // =========================
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(assembly));

        // =========================
        // ✅ FluentValidation (ONLY ONCE)
        // =========================
        services.AddValidatorsFromAssembly(assembly);

        // =========================
        // ✅ Pipeline Behaviors (ORDER MATTERS)
        // =========================

        // 1. Validation FIRST
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        // 2. Logging SECOND
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        // 3. Transaction LAST
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

        return services;
    }
}