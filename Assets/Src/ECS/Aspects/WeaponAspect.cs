using Unity.Entities;

namespace TestRPG.ECS
{
    public readonly partial struct WeaponAspect : IAspect
    {
        public readonly EnabledRefRW<Weapon> IsEnabled;
    }
}