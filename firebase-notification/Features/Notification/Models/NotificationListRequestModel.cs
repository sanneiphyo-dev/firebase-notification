namespace FirebaseNotification.Features.Notification.Models;

public class NotificationListRequestModel
{
    public string? SearchKeyword { get; set; }
    public int PageNo { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
