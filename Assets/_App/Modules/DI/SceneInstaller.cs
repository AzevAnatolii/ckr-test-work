using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SceneInstaller : MonoInstaller
{
    [SerializeField] private ClickerView _clickerView;
    [SerializeField] private WeatherView _weatherView;
    [SerializeField] private DogsView _dogsView;
    
    public override void InstallBindings()
    {
        Container.QueueForInject(_clickerView);
        Container.QueueForInject(_weatherView);
        Container.QueueForInject(_dogsView);
    }
}
