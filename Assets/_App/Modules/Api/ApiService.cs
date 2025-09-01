using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

public class ApiService
{
    private readonly Queue<ApiRequest> _requestQueue = new Queue<ApiRequest>();
    private readonly HashSet<string> _queuedRequestIds = new HashSet<string>();
    private ApiRequest _currentRequest;
    private bool _isProcessing;
    
    private CancellationTokenSource _serviceCancellationTokenSource;

    [Inject]
    public ApiService()
    {
        _serviceCancellationTokenSource = new CancellationTokenSource();
        _ = ProcessRequestQueue(_serviceCancellationTokenSource.Token);
    }

    public UniTask<ApiResponse> SendRequest(string url, string method = "GET", string postData = null)
    {
        var request = new ApiRequest(url, method, postData);
        
        lock (_requestQueue)
        {
            _requestQueue.Enqueue(request);
            _queuedRequestIds.Add(request.Id);
        }

        return request.CompletionSource.Task;
    }

    public bool CancelRequest(string requestId)
    {
        lock (_requestQueue)
        {
            // check if it's the current request
            if (_currentRequest != null && _currentRequest.Id == requestId)
            {
                _currentRequest.Cancel();
                return true;
            }

            // check if it's in the queue
            if (_queuedRequestIds.Contains(requestId))
            {
                var tempQueue = new Queue<ApiRequest>();
                bool found = false;

                while (_requestQueue.Count > 0)
                {
                    var request = _requestQueue.Dequeue();
                    if (request.Id == requestId)
                    {
                        request.Cancel();
                        _queuedRequestIds.Remove(requestId);
                        found = true;
                    }
                    else
                    {
                        tempQueue.Enqueue(request);
                    }
                }

                while (tempQueue.Count > 0)
                {
                    _requestQueue.Enqueue(tempQueue.Dequeue());
                }

                return found;
            }
        }

        return false;
    }

    public void CancelAllRequests()
    {
        lock (_requestQueue)
        {
            // cancel current request
            _currentRequest?.Cancel();

            // cancel all queued requests
            while (_requestQueue.Count > 0)
            {
                var request = _requestQueue.Dequeue();
                request.Cancel();
            }

            _queuedRequestIds.Clear();
        }
    }

    private async UniTaskVoid ProcessRequestQueue(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            ApiRequest request = null;

            lock (_requestQueue)
            {
                if (_requestQueue.Count > 0 && !_isProcessing)
                {
                    request = _requestQueue.Dequeue();
                    _queuedRequestIds.Remove(request.Id);
                    _currentRequest = request;
                    _isProcessing = true;
                }
            }

            if (request != null)
            {
                await ExecuteRequest(request);

                lock (_requestQueue)
                {
                    _currentRequest = null;
                    _isProcessing = false;
                }
            }
            else
            {
                await UniTask.Yield();
            }
        }
    }

    private async UniTask ExecuteRequest(ApiRequest request)
    {
        try
        {
            using (var webRequest = new UnityWebRequest(request.Url, request.Method))
            {
                if (!string.IsNullOrEmpty(request.PostData))
                {
                    byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(request.PostData);
                    webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                    webRequest.SetRequestHeader("Content-Type", "application/json");
                }

                webRequest.downloadHandler = new DownloadHandlerBuffer();

                var operation = webRequest.SendWebRequest();
                
                // wait for completion or cancellation
                while (!operation.isDone && !request.CancellationTokenSource.Token.IsCancellationRequested)
                {
                    await UniTask.Yield();
                }

                if (request.CancellationTokenSource.Token.IsCancellationRequested)
                {
                    webRequest.Abort();
                    request.CompletionSource.TrySetCanceled();
                    return;
                }

                ApiResponse response;
                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    response = ApiResponse.Success(webRequest.downloadHandler.text, webRequest.responseCode);
                }
                else
                {
                    response = ApiResponse.Failure(webRequest.error, webRequest.responseCode);
                }

                request.CompletionSource.TrySetResult(response);
            }
        }
        catch (Exception ex)
        {
            request.CompletionSource.TrySetException(ex);
        }
    }

    public void Dispose()
    {
        _serviceCancellationTokenSource?.Cancel();
        _serviceCancellationTokenSource?.Dispose();
        CancelAllRequests();
    }
}
