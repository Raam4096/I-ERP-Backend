namespace iERP.Application.Abstractions.Notifications;

public interface IWhatsAppSender
{
    Task SendAsync(string toPhoneNumber, string message, CancellationToken cancellationToken = default);
}
