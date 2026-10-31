namespace HomeServices.Application.Common;

/// <summary>
/// نتيجة عملية تُرجع قيمة عند النجاح، أو رسائل خطأ جاهزة للعرض عند الفشل.
/// </summary>
public sealed class Result<T>
{
    private Result(bool succeeded, T? value, IReadOnlyList<string> errors)
    {
        Succeeded = succeeded;
        Value = value;
        Errors = errors;
    }

    public bool Succeeded { get; }
    public T? Value { get; }
    public IReadOnlyList<string> Errors { get; }

    public static Result<T> Success(T value) => new(true, value, []);

    public static Result<T> Failure(params string[] errors) => new(false, default, errors);
}
