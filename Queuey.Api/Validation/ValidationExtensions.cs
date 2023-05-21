using FluentValidation;
using Noppes.Queuey.Api.Models;

namespace Noppes.Queuey.Api.Validation;

public static class ValidationExtensions
{
    public static IServiceCollection RegisterValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<DequeueParametersModel>, DequeueParametersModelValidator>();
        services.AddScoped<IValidator<ICollection<EnqueueItemModel>>, EnqueueItemCollectionModelValidator>();

        return services;
    }
}
