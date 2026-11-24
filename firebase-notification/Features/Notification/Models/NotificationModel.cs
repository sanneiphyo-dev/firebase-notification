using FirebaseNotification.Enums;

namespace FirebaseNotification.Features.Notification.Models;

public class NotificationModel
{
    public string NotificationId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string TargetType { get; set; } = "Topic"; // Topic or Token
    public string Target { get; set; } = "Announcement";
    public string? PayloadJson { get; set; }
    public string Status { get; set; } = "Success";
    public string? MessageId { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow;
}

public class NotificationListingModel
{
    public int TotalCount { get; set; }
    public List<NotificationModel> NotificationList { get; set; } = [];
}
