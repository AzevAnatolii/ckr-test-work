using System;
using System.Collections;
using System.Collections.Generic;
using Doozy.Engine.UI;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class StateView : MonoBehaviour
{
    public UIToggle ClickerToggle;
    public UIToggle WeatherToggle;
    public UIToggle DogsToggle;
    
    [SerializeField] private UIView _clickerView;
    [SerializeField] private UIView _weatherView;
    [SerializeField] private UIView _dogsView;
    
    [Inject] private StateService _service;
    
    void Awake()
    {
        _service.Initialize(this);
    }
    
    public void ShowClicker()
    {
        _clickerView.Show();
    }
    
    public void ShowWeather()
    {
        _weatherView.Show();
    }
    
    public void ShowDogs()
    {
        _dogsView.Show();
    }
    
    public void HideClicker()
    {
        _clickerView.Hide();
    }
    
    public void HideWeather()
    {
        _weatherView.Hide();
    }
    
    public void HideDogs()
    {
        _dogsView.Hide();
    }
}
