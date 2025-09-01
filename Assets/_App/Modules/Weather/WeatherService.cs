using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class WeatherService
{
    private const string WEATHER_API_URL = "https://api.weather.gov/gridpoints/TOP/32,81/forecast";
    private const float REQUEST_INTERVAL = 5f;
    
    private readonly ApiService _apiService;
    private readonly StateService _stateService;
    
    private WeatherView _view;
    private CancellationTokenSource _weatherCancellationTokenSource;
    private CancellationTokenSource _currentRequestCancellationTokenSource;
    private float _requestTimer;
    private bool _isInitialized;

    [Inject]
    public WeatherService(ApiService apiService, StateService stateService)
    {
        _apiService = apiService;
        _stateService = stateService;
    }

    public void Initialize(WeatherView weatherView)
    {
        _view = weatherView;
        _isInitialized = true;
        
        _stateService.OnStateChanged += OnStateChanged;
        
        _weatherCancellationTokenSource = new CancellationTokenSource();
        _ = WeatherUpdateLoop(_weatherCancellationTokenSource.Token);
    }

    private void OnStateChanged(GameState newState)
    {
        if (newState == GameState.Weather)
        {
            _requestTimer = 0f;
            _ = FetchWeather();
        }
        else
        {
            CancelCurrentWeatherRequest();
            _requestTimer = 0f;
        }
    }

    private async UniTaskVoid WeatherUpdateLoop(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested && _isInitialized)
        {
            await UniTask.Yield();
            
            if (_stateService.CurrentState == GameState.Weather)
            {
                _requestTimer += Time.deltaTime;
                
                if (_requestTimer >= REQUEST_INTERVAL)
                {
                    _requestTimer = 0f;
                    _ = FetchWeather();
                }
            }

        }
    }

    private async UniTaskVoid FetchWeather()
    {
        CancelCurrentWeatherRequest();
        
        _currentRequestCancellationTokenSource = new CancellationTokenSource();
        
        try
        {
            var response = await _apiService.SendRequest(WEATHER_API_URL);
            
            if (_currentRequestCancellationTokenSource.Token.IsCancellationRequested)
            {
                return;
            }
            
            _currentRequestCancellationTokenSource = null;
            
            if (response.IsSuccess)
            {
                ParseAndDisplayWeather(response.Data);
            }
            else
            {
                Debug.LogError($"weather request failed: {response.Error}");
            }
        }
        catch (OperationCanceledException)
        {
            // request was cancelled, this is expected
        }
        catch (Exception ex)
        {
            Debug.LogError($"weather request error: {ex.Message}");
        }
    }

    private void ParseAndDisplayWeather(string jsonData)
    {
        try
        {
            var weatherData = JsonUtility.FromJson<WeatherData>(jsonData);
            
            if (weatherData?.properties?.periods != null && weatherData.properties.periods.Length > 0)
            {
                var currentPeriod = weatherData.properties.periods[0];
                var temperature = currentPeriod.temperature;
                
                if (_view != null)
                {
                    _view.SetWeatherText(temperature);
                    
                    if (!string.IsNullOrEmpty(currentPeriod.icon))
                    {
                        _ = LoadWeatherIcon(currentPeriod.icon);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"failed to parse weather data: {ex.Message}");
        }
    }

    private void CancelCurrentWeatherRequest()
    {
        if (_currentRequestCancellationTokenSource != null)
        {
            _currentRequestCancellationTokenSource.Cancel();
            _currentRequestCancellationTokenSource.Dispose();
            _currentRequestCancellationTokenSource = null;
        }
    }

    private async UniTaskVoid LoadWeatherIcon(string iconUrl)
    {
        try
        {
            using (var request = UnityEngine.Networking.UnityWebRequestTexture.GetTexture(iconUrl))
            {
                var operation = request.SendWebRequest();
                
                while (!operation.isDone)
                {
                    await UniTask.Yield();
                }
                
                if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
                {
                    var texture = ((UnityEngine.Networking.DownloadHandlerTexture)request.downloadHandler).texture;
                    var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
                    _view?.SetWeatherIcon(sprite);
                }
                else
                {
                    Debug.LogError($"failed to load weather icon: {request.error}");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"failed to load weather icon: {ex.Message}");
        }
    }

    public void Dispose()
    {
        if (_stateService != null)
        {
            _stateService.OnStateChanged -= OnStateChanged;
        }
        
        _weatherCancellationTokenSource?.Cancel();
        _weatherCancellationTokenSource?.Dispose();
        CancelCurrentWeatherRequest();
        _isInitialized = false;
    }
}
