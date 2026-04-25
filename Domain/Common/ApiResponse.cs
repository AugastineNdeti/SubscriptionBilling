namespace SubscriptionBilling.Domain.Common
{
    public record ApiResponse<T>(
    bool Success,
    string Message,
    T? Data = default,
    List<string>? Errors = null);
}
