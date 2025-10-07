using Unity.Entities;
using UnityEngine;

namespace TestRPG.ECS
{
    public class WeaponRootAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject rootObject;
        
        public class Baker : Baker<WeaponRootAuthoring>
        {
            public override void Bake(WeaponRootAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new WeaponRoot
                {
                    RootEntity = GetEntity(authoring.rootObject, TransformUsageFlags.Dynamic)
                });
            }
        }
    }

    public struct WeaponRoot : IComponentData
    {
        public Entity RootEntity;
    }
}