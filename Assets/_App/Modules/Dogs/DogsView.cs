using System.Collections.Generic;
using Doozy.Engine.UI;
using UnityEngine;
using Zenject;

public class DogsView : MonoBehaviour
{
    [SerializeField] private GameObject _scrollView;
    [SerializeField] private GameObject _preloader;
    [SerializeField] private RectTransform _dogsItemConteiner;
    [SerializeField] private GameObject _dogsItemPrefab;
    
    [Inject] private DogsService _service;
    
    private DogScrollItemPool _itemPool;

    void Awake()
    {
        _itemPool = new DogScrollItemPool(_dogsItemPrefab, _dogsItemConteiner);
        _service.Initialize(this);
    }
    
    public void ShowBreedsLoader()
    {
        _preloader.SetActive(true);
        _scrollView.SetActive(false);
    }
    
    public void HideBreedsLoader()
    {
        _preloader.SetActive(false);
        _scrollView.SetActive(true);
    }
    
    public void DisplayBreeds(DogBreed[] breeds)
    {
        HideBreedsLoader();
        ClearBreedsList();
        
        for (int i = 0; i < breeds.Length; i++)
        {
            var breed = breeds[i];
            var item = _itemPool.Get();
            
            if (item != null)
            {
                item.SetupBreed(i + 1, breed, OnBreedClicked);
            }
        }
    }
    
    public void ShowFactsLoader(string breedId)
    {
        var item = _itemPool.GetActiveItems().Find(x => x.BreedId == breedId);
        item?.ShowLoader();
    }
    
    public void HideFactsLoader(string breedId)
    {
        var item = _itemPool.GetActiveItems().Find(x => x.BreedId == breedId);
        item?.HideLoader();
    }
    
    public void HideAllFactsLoaders()
    {
        var activeItems = _itemPool.GetActiveItems();
        foreach (var item in activeItems)
        {
            item.HideLoader();
        }
    }
    
    public void ShowBreedPopup(string breedName, string description)
    {
        var popup = UIPopupManager.ShowPopup("DogsPopup", false, false);
        popup.GetComponent<DogPopup>().Show(breedName, description);
    }
    
    private void OnBreedClicked(string breedId, string breedName)
    {
        _service.FetchBreedFacts(breedId, breedName);
    }
    
    private void ClearBreedsList()
    {
        _itemPool?.ReturnAll();
    }
    
    private void OnDestroy()
    {
        _itemPool?.Clear();
    }
}
