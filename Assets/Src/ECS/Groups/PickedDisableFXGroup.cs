using Unity.Entities;
using Unity.Transforms;

namespace TestRPG.ECS
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial class PickedDisableFXGroup : ComponentSystemGroup
    {
        
    }
}