using System.Net;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace MinApi.Validation;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, IValidateable
    where TResponse : IResult
{
    private readonly IValidator<TRequest> _validator;

    public ValidationBehavior(IValidator<TRequest> validator)
    {
        _validator = validator;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        ValidationResult result = _validator.Validate(request);

        if (!result.IsValid)
        {
            return (TResponse)Results.ValidationProblem(
                errors: result.ToDictionary(),
                title: "Invalid Request",
                statusCode: (int)HttpStatusCode.UnprocessableEntity,
                type: "https://httpstatuses.com/422"
            );
        }

        return await next();
    }
}

public static class ValidationExtensions
{
    public static IDictionary<string, string[]> ToDictionary(this ValidationResult validationResult)
    => validationResult.Errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.ErrorMessage).ToArray()
            );
}
