using UnityEngine;

namespace TestRPG
{
    public class PlayerObject
    {
        public Transform PlayerTransform { get; }
        public Camera PlayerCamera { get; }

        public PlayerObject(Transform playerTransform, Camera playerCamera)
        {
            PlayerTransform = playerTransform;
            PlayerCamera = playerCamera;
            
            playerTransform.gameObject.SetActive(false);
        }
    }
}