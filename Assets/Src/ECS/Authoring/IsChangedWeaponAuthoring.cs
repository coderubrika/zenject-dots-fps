using Unity.Entities;
using UnityEngine;

namespace TestRPG.ECS
{
    public class IsChangedWeaponAuthoring : MonoBehaviour
    {
        public class Baker : Baker<IsChangedWeaponAuthoring>
        {
            public override void Bake(IsChangedWeaponAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new IsChangedWeapon());
                SetComponentEnabled<IsChangedWeapon>(entity, false);
            }
        }
    }
    
    public struct IsChangedWeapon : IComponentData, IEnableableComponent {}
}