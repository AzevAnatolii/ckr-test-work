using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class UserDataService
{
    private const string CURRENT_ENERGY_COUNT_KEY = "CurrentEnergyCount";
    private const string COIN_COUNT_KEY = "CoinCount";
    
    private readonly GameConfig _gameConfig;
    
    public int MaxEnergyCount => _gameConfig.MaxEnergyCount;
    public int CurrentEnergyCount => _currentEnergyCount;
    public int CoinCount => _coinCount;
    public int EnergyRestoreAmount => _gameConfig.EnergyRestoreAmount;
    public float EnergyRestoreInterval => _gameConfig.EnergyRestoreInterval;
    public int EnergyCostPerClick => _gameConfig.EnergyCostPerClick;
    public int CoinRewardPerClick => _gameConfig.CoinRewardPerClick;
    public float AutoCollectInterval => _gameConfig.AutoCollectInterval;
    
    private int _currentEnergyCount;
    private int _coinCount;

    [Inject]
    public UserDataService(GameConfig gameConfig)
    {
        _gameConfig = gameConfig;
        
        if (!PlayerPrefs.HasKey(CURRENT_ENERGY_COUNT_KEY))
        {
            _currentEnergyCount = _gameConfig.MaxEnergyCount;
            PlayerPrefs.SetInt(CURRENT_ENERGY_COUNT_KEY, _currentEnergyCount);
        }
        else
        {
            _currentEnergyCount = PlayerPrefs.GetInt(CURRENT_ENERGY_COUNT_KEY, 0);
        }
        _coinCount = PlayerPrefs.GetInt(COIN_COUNT_KEY, 0);
    }

    public void IncreaseCoinCount(int coinCount)
    {
        _coinCount += coinCount;
        PlayerPrefs.SetInt(COIN_COUNT_KEY, _coinCount);
    }
    
    public void IncreaseCurrentEnergyCount(int energyCount)
    {
        if (_currentEnergyCount + energyCount > _gameConfig.MaxEnergyCount)
        {
            _currentEnergyCount = _gameConfig.MaxEnergyCount;
            return;
        }
        
        _currentEnergyCount += energyCount;
        PlayerPrefs.SetInt(CURRENT_ENERGY_COUNT_KEY, _currentEnergyCount);
    }
    
    public void DecreaseCurrentEnergyCount(int energyCount)
    {
        if (_currentEnergyCount < energyCount)
        {
            _currentEnergyCount = 0;
            return;
        }
        
        _currentEnergyCount -= energyCount;
        PlayerPrefs.SetInt(CURRENT_ENERGY_COUNT_KEY, _currentEnergyCount);
    }
}
