using FirebaseNotification.Enums;

namespace FirebaseNotification.Services.Firebase;

public interface IFirebaseService
{
    Task<string> FirebaseNotification(
        string token,
        string title,
        string body,
        EnumFirebaseNotificationType notificationType = EnumFirebaseNotificationType.Notification,
        object? data = null);

    Task<string> FirebaseNotificationByTopic(
        string topic,
        string title,
        string body,
        EnumFirebaseNotificationType notificationType = EnumFirebaseNotificationType.Announcement,
        object? data = null);

    Task<string> FirebaseNotificationByUserType(
        EnumNotiUserType notiUserType,
        string title,
        string body,
        object? data = null);
}
