using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Clicker,
    Weather,
    Dogs
}

public class StateService
{
    public GameState CurrentState = GameState.Clicker;
    private StateView _view;
    
    public event Action<GameState> OnStateChanged;

    public StateService()
    {
        
    }

    public void Initialize(StateView stateView)
    {
        _view = stateView;
        UIButtonUtil.AddOnValueChangedHandler(_view.ClickerToggle, OnClickClickerToggle);
        UIButtonUtil.AddOnValueChangedHandler(_view.WeatherToggle, OnClickWeatherToggle);
        UIButtonUtil.AddOnValueChangedHandler(_view.DogsToggle, OnClickDogsToggle);
    }

    private void OnClickDogsToggle(bool value)
    {
        if (value && CurrentState != GameState.Dogs)
        {
            HideCurrentView();
            CurrentState = GameState.Dogs;
            OnStateChanged?.Invoke(CurrentState);
            _view.ShowDogs();
        }
    }

    private void OnClickWeatherToggle(bool value)
    {
        if (value && CurrentState != GameState.Weather)
        {
            HideCurrentView();
            CurrentState = GameState.Weather;
            OnStateChanged?.Invoke(CurrentState);
            _view.ShowWeather();
        }
    }

    private void OnClickClickerToggle(bool value)
    {
        if (value && CurrentState != GameState.Clicker)
        {
            HideCurrentView();
            CurrentState = GameState.Clicker;
            OnStateChanged?.Invoke(CurrentState);
            _view.ShowClicker();
        }
    }

    private void HideCurrentView()
    {
        switch (CurrentState)
        {
            case GameState.Clicker:
                _view.HideClicker();
                break;
            case GameState.Weather:
                _view.HideWeather();
                break;
            case GameState.Dogs:
                _view.HideDogs();
                break;
        }
    }
}
