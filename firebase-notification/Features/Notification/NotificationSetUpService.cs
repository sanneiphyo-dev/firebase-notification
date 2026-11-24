using FirebaseNotification.Enums;
using FirebaseNotification.Features.Notification.Models;
using FirebaseNotification.Services;
using FirebaseNotification.Services.Firebase;
using FirebaseNotification.Shared.Models;

namespace FirebaseNotification.Features.Notification;

public class NotificationSetUpService
{
    private readonly ILogger<NotificationSetUpService> _logger;
    private readonly IFirebaseService _firebaseService;
    private readonly NotificationStore _notificationStore;

    public NotificationSetUpService(
        ILogger<NotificationSetUpService> logger,
        IFirebaseService firebaseService,
        NotificationStore notificationStore)
    {
        _logger = logger;
        _firebaseService = firebaseService;
        _notificationStore = notificationStore;
    }

    public async Task<Result<NotificationListingModel>> GetList(NotificationListRequestModel reqModel)
    {
        _logger.LogInformation("[FirebaseNotification] GetList => Process started. PageNo={PageNo}, PageSize={PageSize}", reqModel.PageNo, reqModel.PageSize);
        try
        {
            var (totalCount, items) = _notificationStore.GetPaged(reqModel.PageNo, reqModel.PageSize, reqModel.SearchKeyword);

            var model = new NotificationListingModel
            {
                TotalCount = totalCount,
                NotificationList = items
            };

            _logger.LogInformation("[FirebaseNotification] GetList => Process completed successfully. Total={Total}", totalCount);
            return await Task.FromResult(Result<NotificationListingModel>.Success(model));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[FirebaseNotification] GetList => Error: {Message}", ex.Message);
            return Result<NotificationListingModel>.Error(ex);
        }
    }

    public async Task<Result<NotificationModel>> Create(NotificationCreateReqModel reqModel)
    {
        _logger.LogInformation("[FirebaseNotification] Create => Started. Title={Title}, TargetType={TargetType}", reqModel.Title, reqModel.TargetType);

        if (string.IsNullOrWhiteSpace(reqModel.Title))
        {
            _logger.LogWarning("[FirebaseNotification] Create => Title is required.");
            return Result<NotificationModel>.Error("Notification Title is required.");
        }

        if (string.IsNullOrWhiteSpace(reqModel.Content))
        {
            _logger.LogWarning("[FirebaseNotification] Create => Content is required.");
            return Result<NotificationModel>.Error("Notification Content is required.");
        }

        var notification = new NotificationModel
        {
            NotificationId = Guid.NewGuid().ToString("N")[..10],
            Title = reqModel.Title.Trim(),
            Content = reqModel.Content.Trim(),
            TargetType = reqModel.TargetType.ToString(),
            Target = string.IsNullOrWhiteSpace(reqModel.Target) ? "test-topic" : reqModel.Target.Trim(),
            PayloadJson = reqModel.PayloadJson,
            CreatedDateTime = DateTime.UtcNow
        };

        try
        {
            string messageId;
            if (reqModel.TargetType == EnumNotiTargetType.Token)
            {
                messageId = await _firebaseService.FirebaseNotification(
                    notification.Target,
                    notification.Title,
                    notification.Content,
                    reqModel.NotiType,
                    notification.PayloadJson);
            }
            else
            {
                messageId = await _firebaseService.FirebaseNotificationByTopic(
                    notification.Target,
                    notification.Title,
                    notification.Content,
                    EnumFirebaseNotificationType.Announcement,
                    notification.PayloadJson);
            }

            notification.Status = "Success";
            notification.MessageId = messageId;
            _notificationStore.Add(notification);

            _logger.LogInformation("[FirebaseNotification] Create => Completed successfully. MessageId={MessageId}", messageId);
            return Result<NotificationModel>.Success(notification);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[FirebaseNotification] Create => Failed to send to FCM: {Message}", ex.Message);
            notification.Status = "Failed";
            notification.ErrorMessage = ex.Message;
            _notificationStore.Add(notification);
            return Result<NotificationModel>.Error(notification, "ME#FCM");
        }
    }

    public async Task<Result<NotificationModel>> GetById(NotificationDetailReqModel reqModel)
    {
        _logger.LogInformation("[FirebaseNotification] GetById => Id={Id}", reqModel.NotificationId);
        try
        {
            var item = _notificationStore.GetById(reqModel.NotificationId);
            if (item is null)
            {
                _logger.LogWarning("[FirebaseNotification] GetById => NotFound: {Id}", reqModel.NotificationId);
                return Result<NotificationModel>.Error("Notification not found.");
            }

            return await Task.FromResult(Result<NotificationModel>.Success(item));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[FirebaseNotification] GetById => Error: {Message}", ex.Message);
            return Result<NotificationModel>.Error(ex);
        }
    }

    public async Task<Result<string>> Delete(NotificationDetailReqModel reqModel)
    {
        _logger.LogInformation("[FirebaseNotification] Delete => Id={Id}", reqModel.NotificationId);
        try
        {
            bool removed = _notificationStore.Delete(reqModel.NotificationId);
            if (!removed)
            {
                return Result<string>.Error("Notification not found.");
            }

            return await Task.FromResult(Result<string>.Success("Deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[FirebaseNotification] Delete => Error: {Message}", ex.Message);
            return Result<string>.Error(ex);
        }
    }

    public async Task<Result<NotificationModel>> Resend(NotificationDetailReqModel reqModel)
    {
        _logger.LogInformation("[FirebaseNotification] Resend => Id={Id}", reqModel.NotificationId);
        try
        {
            var existing = _notificationStore.GetById(reqModel.NotificationId);
            if (existing is null)
            {
                return Result<NotificationModel>.Error("Notification not found.");
            }

            var createReq = new NotificationCreateReqModel
            {
                Title = existing.Title,
                Content = existing.Content,
                TargetType = existing.TargetType == "Token" ? EnumNotiTargetType.Token : EnumNotiTargetType.Topic,
                Target = existing.Target,
                PayloadJson = existing.PayloadJson
            };

            return await Create(createReq);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[FirebaseNotification] Resend => Error: {Message}", ex.Message);
            return Result<NotificationModel>.Error(ex);
        }
    }
}
