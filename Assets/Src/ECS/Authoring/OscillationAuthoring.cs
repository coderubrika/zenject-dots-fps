using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace TestRPG.ECS
{
    public class OscillationAuthoring : MonoBehaviour
    {
        [SerializeField] private float amplitude;
        [SerializeField] private Vector3 axis;
        [SerializeField] private float speed;
        [SerializeField] private bool isDisableByPick;
        
        public class Baker : Baker<OscillationAuthoring>
        {
            public override void Bake(OscillationAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Oscillation
                {
                    Amplitude = authoring.amplitude,
                    Axis = authoring.axis,
                    Speed = authoring.speed,
                    LocalPosition = authoring.transform.localPosition
                });
                
                if (authoring.isDisableByPick)
                    AddComponent<DisableOscillationByPickTag>(entity);
            }
        }
    }

    public struct Oscillation : IComponentData, IEnableableComponent
    {
        public float Amplitude;
        public float Speed;
        public float3 Axis;
        public float3 LocalPosition;
    }
    
    public struct DisableOscillationByPickTag : IComponentData {}
}