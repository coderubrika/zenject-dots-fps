using System;
using Suburb.Inputs;
using Suburb.Utils;
using TestRPG.ECS;
using TestRPG.GameStates;
using TestRPG.Input;
using TestRPG.PlayerDir;
using Unity.Scenes;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace TestRPG.Installers
{
    public class SceneInstaller : MonoInstaller
    {
        [SerializeField] private string screensRootName;
        [SerializeField] private string screensPath;
        [SerializeField] private LoadingModal loadingModalPrefab;
        [SerializeField] private SubScene subScene;
        [SerializeField] private SceneAsset sceneAsset;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Transform playerBody;
        [SerializeField] private InputLayout inputLayoutPrefab;
        
        public override void InstallBindings()
        {
            Container.BindInstance(mainCamera);
            
            Container.Bind<InjectCreator>()
                .AsSingle()
                .NonLazy();
            
            Container.BindInterfacesAndSelfTo<EcsService>()
                .AsSingle()
                .WithArguments(subScene, sceneAsset)
                .NonLazy();
            
            Container.BindInterfacesAndSelfTo<ModalRoot>()
                .AsSingle()
                .NonLazy();
            
            Container.BindInterfacesAndSelfTo<LoadingModalService>()
                .AsSingle()
                .WithArguments(loadingModalPrefab)
                .NonLazy();
            
            Container.Bind<IFactory<Type, BaseScreen>>().To<ScreensFactory>()
                .AsSingle()
                .WithArguments((screensPath, screensRootName));
            
            Container.Bind<ScreenService>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<TouchProvider>().AsSingle();
            Container.BindInterfacesAndSelfTo<MouseProvider>().AsSingle();
            Container.BindInterfacesAndSelfTo<KeyboardInputProvider>().AsSingle();
            Container.BindInterfacesAndSelfTo<TouchResourceDistributor>().AsSingle();
            Container.BindInterfacesAndSelfTo<MouseResourceDistributor>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<LayerOrderer>()
                .AsSingle()
                .NonLazy();
            
            Container.BindInterfacesAndSelfTo<InputLayoutService>()
                .AsSingle()
                .WithArguments(inputLayoutPrefab)
                .NonLazy();
                
            Container.BindInterfacesAndSelfTo<MouseKeyboardInputProvider>().AsSingle();
            Container.Bind<PlayerInputService>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<Startup>()
                .AsSingle()
                .NonLazy();

            Container.Bind<PlayerObject>().AsSingle()
                .WithArguments(playerBody, mainCamera);
            
            Container.Bind<StateFactory<IGameState>>()
                .To<GameStateFactory>()
                .AsSingle();
            
            Container.Bind<PlayerService>().AsSingle();
        }
    }
}