using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace Noppes.Queuey.Api.Validation;

public class ValidationFilter : IAsyncActionFilter
{
    private readonly IServiceProvider _services;

    public ValidationFilter(IServiceProvider services)
    {
        _services = services;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var parameter in context.ActionDescriptor.Parameters)
        {
            if (!context.ActionArguments.TryGetValue(parameter.Name, out var value))
                continue;

            if (value == null)
                continue;

            var validatorType = typeof(IValidator<>).MakeGenericType(parameter.ParameterType);
            var validator = (IValidator?)_services.GetService(validatorType);
            if (validator == null)
                continue;

            var validationContextType = typeof(ValidationContext<>).MakeGenericType(parameter.ParameterType);
            var validationContext = (IValidationContext)Activator.CreateInstance(validationContextType, value)!;

            var validationResult = await validator.ValidateAsync(validationContext);
            if (validationResult.IsValid)
                continue;

            var model = validationResult.Errors
                .GroupBy(x => x.PropertyName)
                .Select(x => new ValidationErrorModel
                {
                    Field = x.Key.StartsWith('_') ? x.Key[1..] : x.Key,
                    Errors = x.Select(y => new ValidationErrorLineModel
                    {
                        Code = y.ErrorCode,
                        Message = y.ErrorMessage
                    }).ToList()
                });

            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.HttpContext.Response.WriteAsJsonAsync(model);

            return;
        }

        await next();
    }
}
