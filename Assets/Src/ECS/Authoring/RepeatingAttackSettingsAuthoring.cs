using Unity.Entities;
using UnityEngine;

namespace TestRPG.ECS
{
    public class RepeatingAttackSettingsAuthoring : MonoBehaviour
    {
        [SerializeField] private float currentTimeSec;
        [SerializeField] private float timeRateSec;
        
        public class Baker : Baker<RepeatingAttackSettingsAuthoring>
        {
            public override void Bake(RepeatingAttackSettingsAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new RepeatingAttackSettings
                {
                    CurrentTimeSec = authoring.currentTimeSec,
                    TimeRateSec = authoring.timeRateSec
                });
            }
        }
    }

    public struct RepeatingAttackSettings : IComponentData
    {
        public float CurrentTimeSec;
        public float TimeRateSec;
    }
}