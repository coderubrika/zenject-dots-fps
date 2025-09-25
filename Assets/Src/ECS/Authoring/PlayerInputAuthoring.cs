using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace TestRPG.ECS
{
    public class PlayerInputAuthoring : MonoBehaviour
    {
        public class Baker : Baker<PlayerInputAuthoring>
        {
            public override void Bake(PlayerInputAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new PlayerInput
                {
                    RotateAxis = float2.zero,
                    IsFire = false,
                    MoveDirectionAndForce = (float2.zero, 0)
                });
            }
        }
    }

    public struct PlayerInput : IComponentData
    {
        public float2 RotateAxis;
        public bool IsFire;
        public (float2 Direction, float Force) MoveDirectionAndForce;
    }
}