using System;
using TestRPG.PlayerDir;
using Unity.Entities;

namespace TestRPG
{
    public class GameContext : IDisposable
    {
        public PlayerData PlayerData { get; private set; }
        public void SetPlayerData(PlayerData value) => PlayerData = value;
        
        public Entity PlayerInputEntity { get; private set; }
        public void SetPlayerInputEntity(Entity entity) => PlayerInputEntity = entity;
        
        public Entity PlayerEntity { get; private set; }
        public void SetPlayerEntity(Entity entity) => PlayerEntity = entity;
        
        public Entity PlayerCameraEntity { get; private set; }
        public void SetPlayerCameraEntity(Entity entity) => PlayerCameraEntity = entity;
        
        public IDisposable PlayerInputBridge { get; private set; }
        public void SetPlayerInputBridge(IDisposable disposable) => PlayerInputBridge = disposable;
        
        public IDisposable PlayerDataBridge { get; private set; }
        public void SetPlayerDataBridge(IDisposable disposable) => PlayerDataBridge = disposable;
        
        public IDisposable PlayerAndCameraTransformBridge { get; private set; }
        public void SetPlayerAndCameraTransformBridge(IDisposable disposable) => PlayerAndCameraTransformBridge = disposable;
        
        public void Dispose()
        {
            PlayerInputEntity = Entity.Null;
            PlayerInputBridge?.Dispose();

            PlayerCameraEntity = Entity.Null;
            PlayerAndCameraTransformBridge?.Dispose();
            
            PlayerDataBridge?.Dispose();
        }
    }
}