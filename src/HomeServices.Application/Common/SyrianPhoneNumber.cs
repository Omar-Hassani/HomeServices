namespace HomeServices.Application.Common;

/// <summary>
/// التحقق من أرقام الهواتف المحمولة السورية وتوحيد صيغتها إلى 09XXXXXXXX.
/// يقبل الصيغ: 0912345678، 912345678، +963912345678، 00963912345678، مع مسافات أو شرطات.
/// </summary>
public static class SyrianPhoneNumber
{
    public static bool TryNormalize(string? input, out string normalized)
    {
        normalized = string.Empty;
        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        var digits = new string(input.Where(char.IsAsciiDigit).ToArray());

        if (digits.StartsWith("00963"))
        {
            digits = digits[5..];
        }
        else if (digits.StartsWith("963"))
        {
            digits = digits[3..];
        }

        if (digits.Length == 9 && digits[0] == '9')
        {
            digits = "0" + digits;
        }

        if (digits.Length != 10 || !digits.StartsWith("09"))
        {
            return false;
        }

        normalized = digits;
        return true;
    }
}
