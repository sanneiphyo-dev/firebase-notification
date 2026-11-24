using System.Text.Json;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using FirebaseNotification.Enums;
using Google.Apis.Auth.OAuth2;

namespace FirebaseNotification.Services.Firebase;

public class FirebaseService : IFirebaseService
{
    private readonly ILogger<FirebaseService> _logger;
    private readonly FirebaseApp _firebaseApp;
    private static readonly Lock InitLock = new();

    public FirebaseService(ILogger<FirebaseService> logger, IWebHostEnvironment env)
    {
        _logger = logger;
        _firebaseApp = InitializeFirebase(env, logger);
    }

    public async Task<string> FirebaseNotification(
        string token,
        string title,
        string body,
        EnumFirebaseNotificationType notificationType = EnumFirebaseNotificationType.Notification,
        object? data = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token, nameof(token));

        try
        {
            _logger.LogInformation("[FirebaseNotification] FirebaseNotification => Sending to token={Token}, title={Title}", token[..Math.Min(15, token.Length)], title);

#pragma warning disable CS0618
            var message = new Message
            {
                Token = token,
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                },
                Data = BuildData(title, body, notificationType, data),
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
                        Sound = "default",
                        ContentAvailable = true
                    }
                }
            };
#pragma warning restore CS0618

            var messaging = FirebaseMessaging.GetMessaging(_firebaseApp);
            string messageId = await messaging.SendAsync(message);

            _logger.LogInformation("[FirebaseNotification] FirebaseNotification => Success. msgId={MessageId}", messageId);
            return messageId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[FirebaseNotification] FirebaseNotification => Error: {Message}", ex.Message);
            throw;
        }
    }

    public async Task<string> FirebaseNotificationByTopic(
        string topic,
        string title,
        string body,
        EnumFirebaseNotificationType notificationType = EnumFirebaseNotificationType.Announcement,
        object? data = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(topic, nameof(topic));

        try
        {
            _logger.LogInformation("[FirebaseNotification] FirebaseNotificationByTopic => Sending to topic={Topic}, title={Title}", topic, title);

            var message = new Message
            {
                Topic = topic,
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                },
                Data = BuildData(title, body, notificationType, data),
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
                        Sound = "default",
                        ContentAvailable = true
                    }
                }
            };

            var messaging = FirebaseMessaging.GetMessaging(_firebaseApp);
            string messageId = await messaging.SendAsync(message);

            _logger.LogInformation("[FirebaseNotification] FirebaseNotificationByTopic => Success. msgId={MessageId}", messageId);
            return messageId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[FirebaseNotification] FirebaseNotificationByTopic => Error: {Message}", ex.Message);
            throw;
        }
    }

    public Task<string> FirebaseNotificationByUserType(
        EnumNotiUserType notiUserType,
        string title,
        string body,
        object? data = null)
    {
        string topic = notiUserType switch
        {
            EnumNotiUserType.All => "Announcement",
            EnumNotiUserType.Wallet => "WalletAnnouncement",
            EnumNotiUserType.Agent => "AgentAnnouncement",
            EnumNotiUserType.Merchant => "MerchantAnnouncement",
            _ => "Announcement"
        };

        return FirebaseNotificationByTopic(topic, title, body, EnumFirebaseNotificationType.Announcement, data);
    }

    private static FirebaseApp InitializeFirebase(IWebHostEnvironment env, ILogger logger)
    {
        if (FirebaseApp.DefaultInstance is { } existingApp)
        {
            return existingApp;
        }

        lock (InitLock)
        {
            if (FirebaseApp.DefaultInstance is { } doubleChecked)
            {
                return doubleChecked;
            }

            string jsonFilePath = ResolveConfigPath(env.ContentRootPath);
            logger.LogInformation("[FirebaseNotification] InitializeFirebase => Using credentials: {Path}", jsonFilePath);

            var credential = CredentialFactory.FromFile<ServiceAccountCredential>(jsonFilePath).ToGoogleCredential();
            return FirebaseApp.Create(new AppOptions
            {
                Credential = credential
            });
        }
    }

    private static string ResolveConfigPath(string contentRoot)
    {
        string[] candidatePaths =
        [
            Path.Combine(contentRoot, "Config", "firebase-adminsdk.json"),
            Path.Combine(AppContext.BaseDirectory, "Config", "firebase-adminsdk.json"),
            Path.Combine(contentRoot, "firebase-adminsdk.json"),
            Path.Combine(AppContext.BaseDirectory, "firebase-adminsdk.json"),
            Path.Combine(Directory.GetCurrentDirectory(), "Config", "firebase-adminsdk.json")
        ];

        foreach (var path in candidatePaths)
        {
            if (File.Exists(path))
            {
                return path;
            }
        }

        return candidatePaths[0];
    }

    private static Dictionary<string, string> BuildData(
        string title,
        string body,
        EnumFirebaseNotificationType type,
        object? data)
    {
        var dict = new Dictionary<string, string>
        {
            ["title"] = title,
            ["body"] = body,
            ["notification_type"] = type.ToString(),
            ["timestamp"] = DateTime.UtcNow.ToString("O")
        };

        if (data is not null)
        {
            dict["payload"] = data is string s ? s : JsonSerializer.Serialize(data);
        }

        return dict;
    }
}
