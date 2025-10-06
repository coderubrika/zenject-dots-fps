using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace TestRPG.ECS
{
    public class WeaponInventoryAuthoring : MonoBehaviour
    {
        public class Baker : Baker<WeaponInventoryAuthoring>
        {
            public override void Bake(WeaponInventoryAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new WeaponInventory
                {
                    SelectedItem = -1,
                    PrevItem = -1
                });
                AddBuffer<WeaponInventorySlot>(entity);
            }
        }
    }

    public struct WeaponInventory : IComponentData
    {
        public int SelectedItem;
        public int PrevItem;
    }

    public struct WeaponInventorySlot : IBufferElementData
    {
        public Entity ItemEntity;
        public FixedString64Bytes ItemId;
    }
}