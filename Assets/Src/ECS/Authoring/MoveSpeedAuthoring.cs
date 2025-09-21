using Unity.Entities;
using UnityEngine;

namespace TestRPG.ECS
{
    public class MoveSpeedAuthoring : MonoBehaviour
    {
        [SerializeField] private float value;
        
        public class Baker : Baker<MoveSpeedAuthoring>
        {
            public override void Bake(MoveSpeedAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new MoveSpeed
                {
                    Value = authoring.value,
                });
            }
        }
    }
    
    public struct MoveSpeed : IComponentData
    {
        public float Value;
    }
}