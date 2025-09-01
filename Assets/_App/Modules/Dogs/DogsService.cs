using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class DogsService
{
    private const string BREEDS_API_URL = "https://dogapi.dog/api/v2/breeds";
    private const string FACTS_API_URL = "https://dogapi.dog/api/v2/facts";
    private const int MAX_BREEDS_COUNT = 10;
    
    private readonly ApiService _apiService;
    private readonly StateService _stateService;
    
    private DogsView _view;
    private CancellationTokenSource _serviceToken;
    private CancellationTokenSource _breedsRequestToken;
    private CancellationTokenSource _factsRequestToken;
    private bool _isInitialized;

    [Inject]
    public DogsService(ApiService apiService, StateService stateService)
    {
        _apiService = apiService;
        _stateService = stateService;
    }
    
    public void Initialize(DogsView dogsView)
    {
        _view = dogsView;
        _isInitialized = true;
        
        _stateService.OnStateChanged += OnStateChanged;
        _serviceToken = new CancellationTokenSource();
        
        if (_stateService.CurrentState == GameState.Dogs)
        {
            _ = FetchBreeds();
        }
    }
    
    private void OnStateChanged(GameState newState)
    {
        if (newState == GameState.Dogs)
        {
            _ = FetchBreeds();
        }
        else
        {
            CancelAllRequests();
        }
    }
    
    private async UniTaskVoid FetchBreeds()
    {
        if (!_isInitialized) return;
        
        CancelBreedsRequest();
        _breedsRequestToken = new CancellationTokenSource();
        
        _view?.ShowBreedsLoader();
        
        try
        {
            var response = await _apiService.SendRequest(BREEDS_API_URL);
            
            if (_breedsRequestToken.Token.IsCancellationRequested) return;
            
            _breedsRequestToken = null;
            
            if (response.IsSuccess)
            {
                ParseAndDisplayBreeds(response.Data);
            }
            else
            {
                Debug.LogError($"breeds request failed: {response.Error}");
                _view?.HideBreedsLoader();
            }
        }
        catch (OperationCanceledException)
        {
            // request was cancelled, this is expected
        }
        catch (Exception ex)
        {
            Debug.LogError($"breeds request error: {ex.Message}");
            _view?.HideBreedsLoader();
        }
    }
    
    public async UniTaskVoid FetchBreedFacts(string breedId, string breedName)
    {
        if (!_isInitialized) return;
        
        CancelFactsRequest();
        _factsRequestToken = new CancellationTokenSource();
        
        _view?.ShowFactsLoader(breedId);
        
        try
        {
            var response = await _apiService.SendRequest($"{FACTS_API_URL}?breed_ids={breedId}");
            
            if (_factsRequestToken.Token.IsCancellationRequested) return;
            
            _factsRequestToken = null;
            _view?.HideFactsLoader(breedId);
            
            if (response.IsSuccess)
            {
                ParseAndShowBreedFacts(response.Data, breedName);
            }
            else
            {
                Debug.LogError($"facts request failed: {response.Error}");
                _view?.ShowBreedPopup(breedName, "No facts available for this breed.");
            }
        }
        catch (OperationCanceledException)
        {
            _view?.HideFactsLoader(breedId);
        }
        catch (Exception ex)
        {
            Debug.LogError($"facts request error: {ex.Message}");
            _view?.HideFactsLoader(breedId);
            _view?.ShowBreedPopup(breedName, "Error loading facts for this breed.");
        }
    }
    
    private void ParseAndDisplayBreeds(string jsonData)
    {
        try
        {
            var breedsResponse = JsonUtility.FromJson<DogBreedsResponse>(jsonData);
            
            if (breedsResponse?.data != null && breedsResponse.data.Length > 0)
            {
                var breedsToShow = Mathf.Min(breedsResponse.data.Length, MAX_BREEDS_COUNT);
                var breeds = new DogBreed[breedsToShow];
                Array.Copy(breedsResponse.data, breeds, breedsToShow);
                
                _view?.DisplayBreeds(breeds);
            }
            else
            {
                Debug.LogWarning("no breeds data received");
                _view?.HideBreedsLoader();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"failed to parse breeds data: {ex.Message}");
            _view?.HideBreedsLoader();
        }
    }
    
    private void ParseAndShowBreedFacts(string jsonData, string breedName)
    {
        try
        {
            var factsResponse = JsonUtility.FromJson<DogFactsResponse>(jsonData);
            
            if (factsResponse?.data != null && factsResponse.data.Length > 0)
            {
                var fact = factsResponse.data[0];
                var description = fact.attributes?.body ?? "No description available.";
                _view?.ShowBreedPopup(breedName, description);
            }
            else
            {
                _view?.ShowBreedPopup(breedName, "No facts available for this breed.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"failed to parse facts data: {ex.Message}");
            _view?.ShowBreedPopup(breedName, "Error parsing breed information.");
        }
    }
    
    private void CancelBreedsRequest()
    {
        if (_breedsRequestToken != null)
        {
            _breedsRequestToken.Cancel();
            _breedsRequestToken.Dispose();
            _breedsRequestToken = null;
        }
    }
    
    private void CancelFactsRequest()
    {
        if (_factsRequestToken != null)
        {
            _factsRequestToken.Cancel();
            _factsRequestToken.Dispose();
            _factsRequestToken = null;
        }
    }
    
    private void CancelAllRequests()
    {
        CancelBreedsRequest();
        CancelFactsRequest();
        _view?.HideBreedsLoader();
        _view?.HideAllFactsLoaders();
    }
    
    public void Dispose()
    {
        if (_stateService != null)
        {
            _stateService.OnStateChanged -= OnStateChanged;
        }
        
        _serviceToken?.Cancel();
        _serviceToken?.Dispose();
        CancelAllRequests();
        _isInitialized = false;
    }
}
