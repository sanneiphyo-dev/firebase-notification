using FirebaseNotification.Enums;

namespace FirebaseNotification.Features.Notification.Models;

public class NotificationCreateReqModel
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public EnumNotiTargetType TargetType { get; set; } = EnumNotiTargetType.Topic;
    public string? Target { get; set; }
    public EnumNotiUserType NotiUserType { get; set; } = EnumNotiUserType.All;
    public EnumFirebaseNotificationType NotiType { get; set; } = EnumFirebaseNotificationType.Notification;
    public string? PayloadJson { get; set; }
}
