using Unity.Entities;
using UnityEngine;

namespace TestRPG.ECS
{
    public class PlayerAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject cameraObject;
        public class Baker : Baker<PlayerAuthoring>
        {
            public override void Bake(PlayerAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Player
                {
                    CameraObject = GetEntity(authoring.cameraObject, TransformUsageFlags.Dynamic)
                });
            }
        }
    }

    public struct Player : IComponentData
    {
        public Entity CameraObject;
    }
}