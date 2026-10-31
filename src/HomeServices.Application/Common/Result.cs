namespace HomeServices.Application.Common;

/// <summary>
/// نتيجة عملية لا تُرجع قيمة: نجاح، أو فشل مع رسائل خطأ جاهزة للعرض للمستخدم.
/// </summary>
public sealed class Result
{
    private Result(bool succeeded, IReadOnlyList<string> errors)
    {
        Succeeded = succeeded;
        Errors = errors;
    }

    public bool Succeeded { get; }
    public IReadOnlyList<string> Errors { get; }

    public static Result Success() => new(true, []);

    public static Result Failure(params string[] errors) => new(false, errors);

    public static Result Failure(IEnumerable<string> errors) => new(false, errors.ToList());
}
