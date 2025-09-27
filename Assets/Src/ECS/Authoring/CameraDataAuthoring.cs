using Unity.Entities;
using UnityEngine;

namespace TestRPG.ECS
{
    public class CameraDataAuthoring : MonoBehaviour {
        public class Baker : Baker<CameraDataAuthoring>
        {
            public override void Bake(CameraDataAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new CameraData());
            }
        }
    }
    
    public struct CameraData : IComponentData
    {
        public float VerticalAngle;
    }
}