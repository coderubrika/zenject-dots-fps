using Unity.Burst;
using Unity.Entities;

namespace TestRPG.ECS
{
    [UpdateAfter(typeof(PickedDisableFXGroup))]
    public partial struct PickedWeaponSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WeaponInventory>();
            state.RequireForUpdate<Player>();
        }
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            Entity playerEntity = SystemAPI.GetSingletonEntity<Player>();
            var playerInventoryBuffer = SystemAPI.GetBuffer<WeaponInventorySlot>(playerEntity);
            var playerInventory = SystemAPI.GetSingletonRW<WeaponInventory>();
            
            foreach (var (
                         inventoryItem,
                         entity) in SystemAPI.Query<
                         RefRW<InventoryItem>>()
                         .WithDisabled<Weapon>()
                         .WithAll<Picked>()
                         .WithEntityAccess())
            {
                SystemAPI.SetComponentEnabled<Picked>(entity, false);
                int index = playerInventoryBuffer.Add(new WeaponInventorySlot
                {
                    ItemEntity = entity,
                    ItemId = inventoryItem.ValueRO.ItemId
                });
                
                inventoryItem.ValueRW.Index = index;
                playerInventory.ValueRW.SelectedItem = index;
                SystemAPI.SetComponentEnabled<IsChangedWeapon>(playerEntity, true);
            }
        }
    }
}