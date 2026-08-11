using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Platform.Notifications.Domain;

public sealed class NotificationLog : AuditableEntity
{

    public Guid? UserId { get; set; }
    public string Channel { get; set; } = "email";
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Status { get; set; } = "pending";
    public string? Error { get; set; }
    public DateTimeOffset? SentAt { get; set; }

}
