using System;
using Suburb.Utils;
using TestRPG.ECS;
using TestRPG.Input;
using UniRx;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace TestRPG.GameStates
{
    public class InitGameState : IGameState
    {
        private readonly PlayerObject playerObject;
        private readonly StartGameSettingsRepository startGameSettingsRepository;
        private readonly PlayerInputService playerInputService;
        private readonly EcsService ecsService;

        private GameContext gameContext;
        private Transform playerTransform;
        private Camera playerCamera;
        
        public InitGameState(
            PlayerObject playerObject, 
            StartGameSettingsRepository startGameSettingsRepository,
            PlayerInputService playerInputService,
            EcsService ecsService)
        {
            this.playerObject = playerObject;
            this.startGameSettingsRepository = startGameSettingsRepository;
            this.playerInputService = playerInputService;
            this.ecsService = ecsService;
        }
        
        public void Apply(StateRouter<IGameState> router, GameContext gameContext)
        {
            this.gameContext = gameContext;
            playerTransform = playerObject.PlayerTransform;
            playerCamera = playerObject.PlayerCamera;

            SetupPlayerTransform();
            playerTransform.gameObject.SetActive(true);
            SetupPlayerInputBridge();
            SetupEcsPlayerTransform();
            SetupEcsPlayerCameraTransform();
            SyncEcsPlayerAndCamera();
        }

        private void SetupPlayerTransform()
        {
            playerTransform.position = startGameSettingsRepository.StartPlayerPosition;
            playerTransform.rotation = Quaternion.Euler(startGameSettingsRepository.StartPlayerRotationAngles);

            playerCamera.transform.position = playerTransform.position + startGameSettingsRepository.PlayerCameraOffset;
            playerCamera.transform.rotation = playerTransform.rotation;
        }
        
        private void SetupPlayerInputBridge()
        {
            IDisposable disposable = Observable.EveryUpdate()
                .ObserveOnMainThread()
                .Subscribe(_ =>
                {
                    ecsService.EntityManager.SetComponentData(gameContext.PlayerInputEntity, new PlayerInput
                    {
                        MoveDirectionAndForce = playerInputService.MoveDirectionAndForce,
                        IsFire = playerInputService.Fire,
                        RotateAxis = playerInputService.RotateAxes
                    });
                });
            
            EntityQuery query = ecsService.EntityManager.CreateEntityQuery(typeof(PlayerInput));
            NativeArray<Entity> entities = query.ToEntityArray(Allocator.Temp);
            
            gameContext.SetPlayerInputEntity(entities[0]);
            playerInputService.Enable();
            query.Dispose();
            entities.Dispose();
            gameContext.SetPlayerInputBridge(disposable);
        }

        private void SetupEcsPlayerTransform()
        {
            EntityQuery query = ecsService.EntityManager.CreateEntityQuery(typeof(Player));
            NativeArray<Entity> entities = query.ToEntityArray(Allocator.Temp);
            gameContext.SetPlayerEntity(entities[0]);
            ecsService.EntityManager.SetComponentData(gameContext.PlayerEntity, new LocalTransform
            {
                Position = playerTransform.position,
                Rotation = playerTransform.rotation,
                Scale = 1
            });
            
            var physicsMass = ecsService.EntityManager.GetComponentData<PhysicsMass>(gameContext.PlayerEntity);
            physicsMass.InverseInertia.x = 0;
            physicsMass.InverseInertia.z = 0;
            physicsMass.InverseInertia.y = 0;
            ecsService.EntityManager.SetComponentData(gameContext.PlayerEntity, physicsMass);
            
            query.Dispose();
            entities.Dispose();
        }

        private void SetupEcsPlayerCameraTransform()
        {
            Player player = ecsService.EntityManager.GetComponentData<Player>(gameContext.PlayerEntity);
            gameContext.SetPlayerCameraEntity(player.CameraObject);

            Vector3 cameraLocalPosition = playerTransform.InverseTransformPoint(playerCamera.transform.position);
            ecsService.EntityManager.SetComponentData(gameContext.PlayerCameraEntity, new LocalTransform
            {
                Position = cameraLocalPosition,
                Rotation = quaternion.identity,
                Scale = 1
            });
        }

        private void SyncEcsPlayerAndCamera()
        {
            IDisposable disposable = Observable.EveryLateUpdate()
                .ObserveOnMainThread()
                .Subscribe(_ =>
                {
                    LocalTransform playerLocalTransform =
                        ecsService.EntityManager.GetComponentData<LocalTransform>(gameContext.PlayerEntity);
                    playerTransform.position = playerLocalTransform.Position;
                    playerTransform.rotation = playerLocalTransform.Rotation;

                    LocalTransform cameraLocalTransform =
                        ecsService.EntityManager.GetComponentData<LocalTransform>(gameContext.PlayerCameraEntity);

                    playerCamera.transform.position = playerTransform.TransformPoint(cameraLocalTransform.Position);
                    playerCamera.transform.rotation = playerTransform.rotation * cameraLocalTransform.Rotation;
                });

            gameContext.SetPlayerAndCameraTransformBridge(disposable);
        }
    }
}