using System;
using Unity.Entities;

namespace TestRPG.ECS
{
    public partial struct ChangeWeaponSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            foreach (var weaponInventoryAspect in SystemAPI.Query<WeaponInventoryAspect>().WithAll<IsChangedWeapon>())
            {
                int weaponIndex = weaponInventoryAspect.WeaponInventory.ValueRO.SelectedItem;
                int prevIndex = weaponInventoryAspect.WeaponInventory.ValueRO.PrevItem;
                
                if (prevIndex == weaponIndex)
                    return;

                if (prevIndex != -1)
                {
                    // скрыть предыдушее оружие
                }

                weaponInventoryAspect.WeaponInventory.ValueRW.PrevItem = weaponIndex;
                var weaponEntity = weaponInventoryAspect.Buffer[weaponIndex].ItemEntity;
                
                


            }
        }
    }
}