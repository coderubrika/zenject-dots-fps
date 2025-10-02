using System;
using Suburb.Utils;
using TestRPG.ECS;
using TestRPG.Input;
using TestRPG.PlayerDir;
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
            playerCamera = playerObject.PlayerCamera;
            SetupPlayerInputBridge();
            SetupEcsPlayerTransform();
            SetupEcsPlayerCameraTransform();
            SyncEcsPlayerAndCamera();
            SetupPlayerData();
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
                Position = startGameSettingsRepository.StartPlayerPosition,
                Rotation = Quaternion.Euler(startGameSettingsRepository.StartPlayerRotationAngles),
                Scale = 1
            });
            
            var physicsMass = ecsService.EntityManager.GetComponentData<PhysicsMass>(gameContext.PlayerEntity);
            physicsMass.InverseInertia = float3.zero;
            ecsService.EntityManager.SetComponentData(gameContext.PlayerEntity, physicsMass);
            
            query.Dispose();
            entities.Dispose();
        }

        private void SetupEcsPlayerCameraTransform()
        {
            Player player = ecsService.EntityManager.GetComponentData<Player>(gameContext.PlayerEntity);
            gameContext.SetPlayerCameraEntity(player.CameraObject);
            PlayerCameraSettings playerCameraSettings =
                ecsService.EntityManager.GetComponentData<PlayerCameraSettings>(gameContext.PlayerEntity);
            
            Vector3 cameraLocalPosition = startGameSettingsRepository.StartPlayerPosition + (Vector3)playerCameraSettings.Offset;
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
                    LocalTransform cameraLocalTransform =
                        ecsService.EntityManager.GetComponentData<LocalTransform>(gameContext.PlayerCameraEntity);

                    playerCamera.transform.position = cameraLocalTransform.Position;
                    playerCamera.transform.rotation = cameraLocalTransform.Rotation;
                });

            gameContext.SetPlayerAndCameraTransformBridge(disposable);
        }

        private void SetupPlayerData()
        {
            Health health = ecsService.EntityManager.GetComponentData<Health>(gameContext.PlayerEntity);
            MoveSpeed moveSpeed = ecsService.EntityManager.GetComponentData<MoveSpeed>(gameContext.PlayerEntity);
            
            gameContext.SetPlayerData(new PlayerData(
                new FloatValue(health.Value, health.Value),
                new FloatValue(moveSpeed.Value, moveSpeed.Value),
                new FloatValue(0, float.MaxValue),
                new FloatValue(0, float.MaxValue)
                ));

            IDisposable disposable = Observable.EveryLateUpdate()
                .Subscribe(_ =>
                {
                    Health currentHealth = ecsService.EntityManager.GetComponentData<Health>(gameContext.PlayerEntity);
                    gameContext.PlayerData.Health.SetValue(currentHealth.Value);
                });
            
            gameContext.SetPlayerDataBridge(disposable);
        }
    }
}