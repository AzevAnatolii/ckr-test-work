using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DogScrollItem : MonoBehaviour
{
    [SerializeField] private Button _dogInfoButton;
    [SerializeField] private TextMeshProUGUI _dogNameText;
    [SerializeField] private GameObject _preloader;
    
    public string BreedId { get; private set; }
    
    private Action<string, string> _onClickCallback;
    private string _breedName;
    
    private void Awake()
    {
        _dogInfoButton.onClick.AddListener(OnButtonClicked);
        _preloader.SetActive(false);
    }
    
    public void SetupBreed(int index, DogBreed breed, Action<string, string> onClickCallback)
    {
        BreedId = breed.id;
        _breedName = breed.attributes?.name ?? "Unknown Breed";
        _onClickCallback = onClickCallback;
        
        _dogNameText.text = _breedName;
    }
    
    public void ShowLoader()
    {
        _preloader.SetActive(true);
    }
    
    public void HideLoader()
    {
        _preloader.SetActive(false);
    }
    
    private void OnButtonClicked()
    {
        _onClickCallback?.Invoke(BreedId, _breedName);
    }
}
