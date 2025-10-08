using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TestRPG.ECS
{
    public partial struct ChangeWeaponSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
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
                SystemAPI.SetComponentEnabled<Weapon>(weaponEntity, true);
                weaponInventoryAspect.IsChanged.ValueRW = false;

                var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
                var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
                var parent = new Parent
                {
                    Value = weaponInventoryAspect.WeaponRoot.ValueRO.RootEntity
                };
                
                if (!SystemAPI.HasComponent<Parent>(weaponEntity))
                    ecb.AddComponent(weaponEntity, parent);
                else
                    ecb.SetComponent(weaponEntity, parent);
                
                ecb.SetComponent(weaponEntity, new LocalTransform
                {
                    Position = float3.zero,
                    Rotation = quaternion.identity,
                    Scale = 1
                });
            }
        }
    }
}