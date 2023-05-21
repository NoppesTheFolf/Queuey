namespace Noppes.Queuey.Api.Validation;

public class ValidationErrorModel
{
    public string? Field { get; set; }

    public ICollection<ValidationErrorLineModel> Errors { get; set; } = null!;
}

public class ValidationErrorLineModel
{
    public string Code { get; set; } = null!;

    public string Message { get; set; } = null!;
}
