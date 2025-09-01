using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class ClickerService
{
    private readonly UserDataService _userDataService;
    private readonly StateService _stateService;
    
    private ClickerView _view;
    private CancellationTokenSource _cancellationTokenSource;
    
    private float _autoCollectIntervalTimer = 0f;
    private float _energyRestoreIntervalTimer = 0f;
    private bool _isEnergyRestoreActive = true;

    [Inject]
    public ClickerService(UserDataService userDataService, StateService stateService)
    {
        _stateService = stateService;
        _userDataService = userDataService;
    }

    public void Initialize(ClickerView view)
    {
        _view = view;
        _view.SetEnergy(_userDataService.CurrentEnergyCount, _userDataService.MaxEnergyCount);
        _view.SetCoin(_userDataService.CoinCount);
        
        _cancellationTokenSource = new CancellationTokenSource();
        _ = TimerLoop(_cancellationTokenSource.Token);
        
        UIButtonUtil.AddOnPointerDownHandler(_view.ClickButton, OnClickButton);
    }

    private void OnClickButton()
    {
        if (_userDataService.CurrentEnergyCount < _userDataService.EnergyCostPerClick)
        {
            _view.SetClickButtonEnabled(false);
            return;
        }
        
        _userDataService.DecreaseCurrentEnergyCount(_userDataService.EnergyCostPerClick);
        _userDataService.IncreaseCoinCount(_userDataService.CoinRewardPerClick);
        
        _view.SetEnergy(_userDataService.CurrentEnergyCount, _userDataService.MaxEnergyCount);
        _view.SetCoin(_userDataService.CoinCount);
        _autoCollectIntervalTimer = 0f;
    }
    
    public void SetEnergyRestoreActive(bool isActive)
    {
        _isEnergyRestoreActive = isActive;
    }

    private async UniTaskVoid TimerLoop(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await UniTask.Yield();

            if (_stateService.CurrentState != GameState.Clicker)
            {
                _autoCollectIntervalTimer = 0f;
                _energyRestoreIntervalTimer = 0f;
                continue;
            }
            
            _autoCollectIntervalTimer += Time.deltaTime;
            if (_autoCollectIntervalTimer >= _userDataService.AutoCollectInterval)
            {
                _autoCollectIntervalTimer = 0f;
                _view.ClickButton.ExecuteClick();
            }
            
            if (_isEnergyRestoreActive)
            {
                _energyRestoreIntervalTimer += Time.deltaTime;
                if (_energyRestoreIntervalTimer >= _userDataService.EnergyRestoreInterval)
                {
                    _energyRestoreIntervalTimer = 0f;
                    RestoreEnergy();
                }
            }
        }
    }
    
    private void RestoreEnergy()
    {
        int energyToRestore = _userDataService.EnergyRestoreAmount;
        int currentEnergy = _userDataService.CurrentEnergyCount;
        int maxEnergy = _userDataService.MaxEnergyCount;
        
        if (currentEnergy < maxEnergy)
        {
            int actualRestore = Mathf.Min(energyToRestore, maxEnergy - currentEnergy);
            _userDataService.IncreaseCurrentEnergyCount(actualRestore);
            _view.SetEnergy(_userDataService.CurrentEnergyCount, _userDataService.MaxEnergyCount);
            
            if (_userDataService.CurrentEnergyCount > 0)
            {
                _view.SetClickButtonEnabled(true);
            }
        }
    }
}
