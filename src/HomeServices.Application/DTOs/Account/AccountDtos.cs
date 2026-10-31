namespace HomeServices.Application.DTOs.Account;

public enum AccountType
{
    Customer = 0,
    Provider = 1
}

public enum LoginStatus
{
    Succeeded,
    InvalidCredentials,
    LockedOut,
    Inactive
}

public sealed record RegisterUserDto(
    string FullName,
    string Email,
    string PhoneNumber,
    string Password,
    AccountType AccountType);

public sealed record UserProfileDto(
    string FullName,
    string Email,
    string? PhoneNumber,
    string? ProfileImagePath,
    DateTime CreatedAt,
    IReadOnlyList<string> Roles);

public sealed record UpdateProfileDto(string FullName, string PhoneNumber);
