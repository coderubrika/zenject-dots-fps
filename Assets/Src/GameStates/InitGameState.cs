using Suburb.Utils;
using UnityEngine;

namespace TestRPG.GameStates
{
    public class InitGameState : IGameState
    {
        private readonly PlayerObject playerObject;
        private readonly StartGameSettingsRepository startGameSettingsRepository;
        
        public InitGameState(
            PlayerObject playerObject, 
            StartGameSettingsRepository startGameSettingsRepository)
        {
            this.playerObject = playerObject;
            this.startGameSettingsRepository = startGameSettingsRepository;
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
            // далее нужно обработать ввод наверное что вообще нужно 
            // вообще по идее нужно задать начальное положение
            // нужно определить как будет меняться физика
            // нужно настроить Authoring таким образом чтобы получить положение и направление player
            // так попробую для начала определить ввод обобщенно
            
        }
    }
}