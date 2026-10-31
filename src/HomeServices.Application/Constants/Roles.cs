namespace HomeServices.Application.Constants;

public static class Roles
{
    public const string Admin = "Admin";
    public const string Customer = "Customer";
    public const string Provider = "Provider";

    public static readonly IReadOnlyList<string> All = [Admin, Customer, Provider];
}
