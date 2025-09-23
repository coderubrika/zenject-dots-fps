using System;
using Unity.Entities;
using Unity.Scenes;
using Zenject;
using UniRx;
using RxUnit = UniRx.Unit;

namespace TestRPG.ECS
{
    public class EcsService : IInitializable
    {
        private World world;
        private Entity sceneEntity;
        
        private readonly Hash128 sceneGUID;
        
        public EcsService(SubScene subScene)
        {
            sceneGUID = subScene.SceneGUID;
        }
        
        public void Initialize()
        {
            world = World.DefaultGameObjectInjectionWorld;
            Disable();
        }
        
        public IObservable<RxUnit> Enable()
        {
            ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop(world);
            return LoadSubScene();
        }

        public void Disable()
        {
            SceneSystem.UnloadScene(world.Unmanaged, sceneEntity, SceneSystem.UnloadParameters.DestroyMetaEntities);
            ScriptBehaviourUpdateOrder.RemoveWorldFromCurrentPlayerLoop(world);
            sceneEntity = Entity.Null;
        }

        private IObservable<RxUnit> LoadSubScene()
        {
            sceneEntity = SceneSystem.LoadSceneAsync(
                world.Unmanaged, 
                sceneGUID
            );

            Subject<RxUnit> subject = new();
            IDisposable updateDisposable = null;
            
            updateDisposable = Observable.EveryUpdate()
                .Subscribe(_ =>
                {
                    SceneSystem.SceneStreamingState state = SceneSystem.GetSceneStreamingState(world.Unmanaged, sceneEntity);
                    switch (state)
                    {
                        case SceneSystem.SceneStreamingState.FailedLoadingSceneHeader:
                        case SceneSystem.SceneStreamingState.LoadedWithSectionErrors:
                        case SceneSystem.SceneStreamingState.Unloading:
                        case SceneSystem.SceneStreamingState.Unloaded:
                        {
                            subject.OnError(new Exception("Error loading sub scene"));
                            updateDisposable?.Dispose();
                            break;
                        }

                        case SceneSystem.SceneStreamingState.LoadedSuccessfully:
                        {
                            subject.OnNext(RxUnit.Default);
                            subject.OnCompleted();
                            updateDisposable?.Dispose();
                            break;
                        }
                    }
                });

            return subject;
        }
    }
}