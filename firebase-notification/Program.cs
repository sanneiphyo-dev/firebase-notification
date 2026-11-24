using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using FirebaseNotification.Models;
using FirebaseNotification.Services;
using Google.Apis.Auth.OAuth2;

var builder = WebApplication.CreateBuilder(args);

// 1. Initialize Firebase Admin SDK
string credentialPath = ResolveCredentialPath("firebase-adminsdk.json");
if (File.Exists(credentialPath))
{
    FirebaseApp.Create(new AppOptions
    {
        Credential = CredentialFactory.FromFile<ServiceAccountCredential>(credentialPath).ToGoogleCredential()
    });
}

builder.Services.AddSingleton<NotificationStore>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

var api = app.MapGroup("/api/notifications");

// READ: Get all notifications
api.MapGet("/", (NotificationStore store) => Results.Ok(store.GetAll()));

// CREATE: Send new notification
api.MapPost("/", async (SendNotificationRequest request, NotificationStore store) =>
{
    if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Body))
    {
        return Results.BadRequest(new { error = "Title and Body are required." });
    }

    var record = new NotificationRecord
    {
        Title = request.Title.Trim(),
        Body = request.Body.Trim(),
        TargetType = request.TargetType?.ToLowerInvariant() == "token" ? "token" : "topic",
        Target = string.IsNullOrWhiteSpace(request.Target) ? "test-topic" : request.Target.Trim(),
        PayloadJson = request.PayloadJson
    };

    await SendToFirebaseAsync(record);
    store.Add(record);
    return Results.Ok(record);
});

// RESEND: Resend an existing notification
api.MapPost("/{id}/resend", async (string id, NotificationStore store) =>
{
    var existing = store.Get(id);
    if (existing is null)
    {
        return Results.NotFound(new { error = "Notification not found." });
    }

    var record = new NotificationRecord
    {
        Title = existing.Title,
        Body = existing.Body,
        TargetType = existing.TargetType,
        Target = existing.Target,
        PayloadJson = existing.PayloadJson
    };

    await SendToFirebaseAsync(record);
    store.Add(record);
    return Results.Ok(record);
});

// DELETE: Delete single notification
api.MapDelete("/{id}", (string id, NotificationStore store) =>
{
    bool deleted = store.Delete(id);
    return deleted ? Results.NoContent() : Results.NotFound();
});

// DELETE: Clear all history
api.MapDelete("/", (NotificationStore store) =>
{
    store.Clear();
    return Results.NoContent();
});

app.Run();

static async Task SendToFirebaseAsync(NotificationRecord record)
{
    try
    {
        Message message;
        var data = new Dictionary<string, string>
        {
            ["time"] = DateTime.UtcNow.ToString("O"),
            ["source"] = "crud-web-tester"
        };

        if (!string.IsNullOrWhiteSpace(record.PayloadJson))
        {
            data["payload"] = record.PayloadJson;
        }

        if (record.TargetType == "token")
        {
#pragma warning disable CS0618 // Token property is marked obsolete in favor of FID by SDK
            message = new Message
            {
                Token = record.Target,
                Notification = new Notification
                {
                    Title = record.Title,
                    Body = record.Body
                },
                Data = data,
                Android = new AndroidConfig
                {
                    Priority = Priority.High,
                    Notification = new AndroidNotification
                    {
                        Sound = "default"
                    }
                },
                Apns = new ApnsConfig
                {
                    Aps = new Aps
                    {
                        Sound = "default"
                    }
                }
            };
#pragma warning restore CS0618
        }
        else
        {
            message = new Message
            {
                Topic = record.Target,
                Notification = new Notification
                {
                    Title = record.Title,
                    Body = record.Body
                },
                Data = data,
                Android = new AndroidConfig
                {
                    Priority = Priority.High,
                    Notification = new AndroidNotification
                    {
                        Sound = "default"
                    }
                },
                Apns = new ApnsConfig
                {
                    Aps = new Aps
                    {
                        Sound = "default"
                    }
                }
            };
        }

        string messageId = await FirebaseMessaging.DefaultInstance.SendAsync(message);
        record.Status = "Success";
        record.MessageId = messageId;
    }
    catch (Exception ex)
    {
        record.Status = "Failed";
        record.ErrorMessage = ex.Message;
    }
}

static string ResolveCredentialPath(string fileName)
{
    string baseDir = AppContext.BaseDirectory;
    string path = Path.Combine(baseDir, fileName);
    if (File.Exists(path)) return path;

    string currentDir = Directory.GetCurrentDirectory();
    path = Path.Combine(currentDir, fileName);
    if (File.Exists(path)) return path;

    path = Path.Combine(currentDir, "firebase-notification", fileName);
    if (File.Exists(path)) return path;

    return fileName;
}
