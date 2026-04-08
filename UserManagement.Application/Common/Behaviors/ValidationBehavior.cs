using FluentValidation;
using MediatR;
using UserManagement.Application.Common;

public class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .Select(f => f.ErrorMessage)
            .ToList();

        if (failures.Any())
        {
            // ✅ HANDLE GENERIC RESULT<T>
            var responseType = typeof(TResponse);

            if (responseType.IsGenericType &&
                responseType.GetGenericTypeDefinition() == typeof(Result<>))
            {
                var failureMethod = responseType.GetMethod("Failure", new[] { typeof(string) });

                if (failureMethod != null)
                {
                    return (TResponse)failureMethod.Invoke(null, new object[]
                    {
                        string.Join(", ", failures)
                    })!;
                }
            }

            // fallback (should not happen ideally)
            throw new ValidationException(string.Join(", ", failures));
        }

        return await next();
    }
}