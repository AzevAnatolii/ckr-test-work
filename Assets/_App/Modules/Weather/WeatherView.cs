using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class WeatherView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _currentWeatherText;
    [SerializeField] private Image _weatherIcon;
    [Inject] private WeatherService _service;

    private void Awake()
    {
        _service.Initialize(this);
    }
    
    public void SetWeatherText(int temperature)
    {
        _currentWeatherText.text = $"{temperature}F";
    }
    
    public void SetWeatherIcon(Sprite sprite)
    {
        _weatherIcon.sprite = sprite;
    }
}
