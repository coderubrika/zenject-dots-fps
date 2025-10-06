using Unity.Entities;
using UnityEngine;

namespace TestRPG.ECS
{
    public class WeaponAuthoring : MonoBehaviour
    {
        [SerializeField] private string itemId;
        
        public class Baker : Baker<WeaponAuthoring>
        {
            public override void Bake(WeaponAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Weapon());
                AddComponent(entity, new InventoryItem
                {
                    ItemId = authoring.itemId,
                    Index = -1
                });
            }
        }
    }
    
    public struct Weapon : IComponentData {}
}