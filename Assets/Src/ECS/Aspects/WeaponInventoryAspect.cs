using Unity.Entities;

namespace TestRPG.ECS
{
    public readonly partial struct WeaponInventoryAspect : IAspect
    {
        public readonly DynamicBuffer<WeaponInventorySlot> Buffer;
        public readonly EnabledRefRW<IsChangedWeapon> IsChanged;
        public readonly RefRW<WeaponInventory> WeaponInventory;
        public readonly RefRO<WeaponRoot> WeaponRoot;
    }
}