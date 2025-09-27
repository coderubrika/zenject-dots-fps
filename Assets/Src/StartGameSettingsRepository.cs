using UnityEngine;

namespace TestRPG
{
    [CreateAssetMenu(fileName = "StartGameSettingsRepository", menuName = "Repositories/StartGameSettingsRepository")]
    public class StartGameSettingsRepository : ScriptableObject
    {
        [SerializeField] private Vector3 startPlayerPosition;
        [SerializeField] private Vector3 startPlayerRotationAngles;
        
        //[SerializeField] private Vector3 playerCameraOffset;

        public Vector3 StartPlayerPosition => startPlayerPosition;

        public Vector3 StartPlayerRotationAngles => startPlayerRotationAngles;

        //public Vector3 PlayerCameraOffset => playerCameraOffset;
    }
}