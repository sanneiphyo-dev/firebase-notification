using FirebaseNotification.Features.Notification.Models;
using FirebaseNotification.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace FirebaseNotification.Features.Notification;

[Route("api/information/notification")]
[ApiController]
public class NotificationController : ControllerBase
{
    private readonly NotificationSetUpService _notificationSetUpService;

    public NotificationController(NotificationSetUpService notificationSetUpService)
    {
        _notificationSetUpService = notificationSetUpService;
    }

    [HttpPost("List")]
    public async Task<IActionResult> GetList([FromBody] NotificationListRequestModel reqModel)
    {
        var result = await _notificationSetUpService.GetList(reqModel);
        return Ok(result);
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] NotificationCreateReqModel reqModel)
    {
        var result = await _notificationSetUpService.Create(reqModel);
        return Ok(result);
    }

    [HttpPost("Detail")]
    public async Task<IActionResult> Detail([FromBody] NotificationDetailReqModel reqModel)
    {
        var result = await _notificationSetUpService.GetById(reqModel);
        return Ok(result);
    }

    [HttpPost("Delete")]
    public async Task<IActionResult> Delete([FromBody] NotificationDetailReqModel reqModel)
    {
        var result = await _notificationSetUpService.Delete(reqModel);
        return Ok(result);
    }

    [HttpPost("Resend")]
    public async Task<IActionResult> Resend([FromBody] NotificationDetailReqModel reqModel)
    {
        var result = await _notificationSetUpService.Resend(reqModel);
        return Ok(result);
    }
}
