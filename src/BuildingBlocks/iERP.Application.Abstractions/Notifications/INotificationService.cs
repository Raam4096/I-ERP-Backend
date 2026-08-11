namespace iERP.Application.Abstractions.Notifications;

public interface INotificationService
{
    Task NotifyAsync(
        Guid tenantId,
        Guid? userId,
        string channel,
        string subject,
        string body,
        CancellationToken cancellationToken = default);
}
