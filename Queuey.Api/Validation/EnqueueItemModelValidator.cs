using FluentValidation;
using Noppes.Queuey.Api.Models;

namespace Noppes.Queuey.Api.Validation;

public class EnqueueItemModelValidator : AbstractValidator<EnqueueItemModel>
{
    public EnqueueItemModelValidator()
    {
        RuleFor(x => x.Message).NotEmpty();
    }
}

public class EnqueueItemCollectionModelValidator : AbstractValidator<ICollection<EnqueueItemModel>>
{
    public EnqueueItemCollectionModelValidator()
    {
        RuleForEach(_ => _)
            .NotEmpty()
            .SetValidator(_ => new EnqueueItemModelValidator());
    }
}
