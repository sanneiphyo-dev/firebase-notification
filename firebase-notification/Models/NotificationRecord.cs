namespace FirebaseNotification.Models;

public sealed class NotificationRecord
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string TargetType { get; set; } = "topic"; // "topic" | "token"
    public string Target { get; set; } = "test-topic";
    public string? PayloadJson { get; set; }
    public string Status { get; set; } = "Pending"; // "Success" | "Failed"
    public string? MessageId { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public sealed record SendNotificationRequest(
    string Title,
    string Body,
    string? TargetType,
    string? Target,
    string? PayloadJson);
