using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

public class ApiRequest
{
    public string Id { get; }
    public string Url { get; }
    public string Method { get; }
    public string PostData { get; }
    public CancellationTokenSource CancellationTokenSource { get; }
    public UniTaskCompletionSource<ApiResponse> CompletionSource { get; }

    public ApiRequest(string url, string method = "GET", string postData = null)
    {
        Id = Guid.NewGuid().ToString();
        Url = url;
        Method = method;
        PostData = postData;
        CancellationTokenSource = new CancellationTokenSource();
        CompletionSource = new UniTaskCompletionSource<ApiResponse>();
    }

    public void Cancel()
    {
        CancellationTokenSource?.Cancel();
        CompletionSource?.TrySetCanceled();
    }
}
