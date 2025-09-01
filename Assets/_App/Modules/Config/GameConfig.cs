using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Game/Game Config")]
public class GameConfig : ScriptableObject
{
    [Header("Energy Settings")]
    [SerializeField] private int _maxEnergyCount = 1000;
    [SerializeField] private int _energyRestoreAmount = 10;
    [SerializeField] private float _energyRestoreInterval = 10f;
    [SerializeField] private int _energyCostPerClick = 1;
    
    [Header("Coin Settings")]
    [SerializeField] private int _coinRewardPerClick = 1;
    
    [Header("Auto Collect Settings")]
    [SerializeField] private float _autoCollectInterval = 3f;
    
    public int MaxEnergyCount => _maxEnergyCount;
    public int EnergyRestoreAmount => _energyRestoreAmount;
    public float EnergyRestoreInterval => _energyRestoreInterval;
    public int EnergyCostPerClick => _energyCostPerClick;
    public int CoinRewardPerClick => _coinRewardPerClick;
    public float AutoCollectInterval => _autoCollectInterval;
}
