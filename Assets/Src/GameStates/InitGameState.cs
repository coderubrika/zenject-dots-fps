using Suburb.Utils;
using TestRPG.Input;
using UniRx;
using UnityEngine;

namespace TestRPG.GameStates
{
    public class InitGameState : IGameState
    {
        private readonly PlayerObject playerObject;
        private readonly StartGameSettingsRepository startGameSettingsRepository;
        private readonly PlayerInputService playerInputService;
        
        public InitGameState(
            PlayerObject playerObject, 
            StartGameSettingsRepository startGameSettingsRepository,
            PlayerInputService playerInputService)
        {
            this.playerObject = playerObject;
            this.startGameSettingsRepository = startGameSettingsRepository;
            this.playerInputService = playerInputService;
        }
        
        public void Apply(StateRouter<IGameState> router, GameContext gameContext)
        {
            Transform playerTransform = playerObject.PlayerTransform;
            Camera playerCamera = playerObject.PlayerCamera;
            
            playerTransform.position = startGameSettingsRepository.StartPlayerPosition;
            playerTransform.rotation = Quaternion.Euler(startGameSettingsRepository.StartPlayerRotationAngles);

            playerCamera.transform.position = playerTransform.position + startGameSettingsRepository.PlayerCameraOffset;
            playerCamera.transform.rotation = playerTransform.rotation;
            
            playerTransform.gameObject.SetActive(true);
    
            playerInputService.Enable();
            
            Observable.EveryUpdate()
                .Subscribe(_ =>
                {
                    if (playerInputService.Fire)
                        this.Log("Fire");
                    
                    if (playerInputService.MoveDirectionAndForce.Direction != Vector2.zero)
                        this.Log($"move {playerInputService.MoveDirectionAndForce}");
                    
                    if (playerInputService.RotateAxes != Vector2.zero)
                        this.Log($"RotateAxes: {playerInputService.RotateAxes}");
                });
        }
    }
}