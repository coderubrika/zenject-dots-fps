using Unity.Collections;
using Unity.Entities;

namespace TestRPG.ECS
{
    public struct InventoryItem : IComponentData
    {
        public FixedString64Bytes ItemId;
        public int Index;
    }
}