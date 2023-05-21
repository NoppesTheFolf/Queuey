using FluentValidation;
using Noppes.Queuey.Api.Models;

namespace Noppes.Queuey.Api.Validation;

public class DequeueParametersModelValidator : AbstractValidator<DequeueParametersModel>
{
    public DequeueParametersModelValidator()
    {
        RuleFor(x => x.Limit).GreaterThan(0);
        RuleFor(x => x.VisibilityDelay).GreaterThanOrEqualTo(TimeSpan.Zero);
    }
}
