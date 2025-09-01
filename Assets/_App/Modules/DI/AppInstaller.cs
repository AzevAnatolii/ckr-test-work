using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class AppInstaller : MonoInstaller
{
    [SerializeField] private GameConfig _gameConfig;
    
    public override void InstallBindings()
    {
        Container.Bind<GameConfig>().FromInstance(_gameConfig).AsSingle();
        Container.Bind<UserDataService>().AsSingle();  
        Container.Bind<ApiService>().AsSingle();
        
        Container.Bind<DogsService>().AsSingle();   
        Container.Bind<WeatherService>().AsSingle();   
        Container.Bind<StateService>().AsSingle();
        
        Container.Bind<ClickerService>().AsSingle().NonLazy();   
    }
}
