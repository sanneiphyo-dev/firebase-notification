using FirebaseNotification.Enums;

namespace FirebaseNotification.Shared.Models;

public class Result<T>
{
    public bool IsSuccess => RespType == EnumRespType.Success;
    public bool IsError => !IsSuccess;
    public EnumRespType RespType { get; set; }
    public string RespCode { get; set; } = string.Empty;
    public string RespDesp { get; set; } = string.Empty;
    public object[]? RespDespParameter { get; set; }
    public T? Data { get; set; }

    public static Result<T> Success(T data, string code = "MS#000")
    {
        return new Result<T> { Data = data, RespCode = code, RespType = EnumRespType.Success };
    }

    public static Result<T> Success(string code = "MS#000")
    {
        return new Result<T> { RespCode = code, RespType = EnumRespType.Success };
    }

    public static Result<T> Error(T data, string code)
    {
        return new Result<T> { Data = data, RespCode = code, RespType = EnumRespType.Error };
    }

    public static Result<T> Error(string message, string code = "ME#999")
    {
        return new Result<T> { RespDesp = message, RespCode = code, RespType = EnumRespType.Error };
    }

    public static Result<T> Error(Exception ex, string code = "ME#999")
    {
        return new Result<T> { RespDesp = ex.Message, RespCode = code, RespType = EnumRespType.Error };
    }
}
