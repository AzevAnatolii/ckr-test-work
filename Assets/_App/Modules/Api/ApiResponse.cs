using UnityEngine.Networking;

public class ApiResponse
{
    public bool IsSuccess { get; }
    public string Data { get; }
    public long ResponseCode { get; }
    public string Error { get; }

    public ApiResponse(bool isSuccess, string data, long responseCode, string error = null)
    {
        IsSuccess = isSuccess;
        Data = data;
        ResponseCode = responseCode;
        Error = error;
    }

    public static ApiResponse Success(string data, long responseCode)
    {
        return new ApiResponse(true, data, responseCode);
    }

    public static ApiResponse Failure(string error, long responseCode)
    {
        return new ApiResponse(false, null, responseCode, error);
    }
}
