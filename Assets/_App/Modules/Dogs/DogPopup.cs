using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DogPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _breedNameText;
    [SerializeField] private TextMeshProUGUI _breedDescriptionText;
    
    public void Show(string breedName, string description)
    {
        _breedNameText.text = breedName;
        _breedDescriptionText.text = description;
    }
}
