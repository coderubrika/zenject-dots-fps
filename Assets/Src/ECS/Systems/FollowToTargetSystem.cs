using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace TestRPG.ECS
{
    [BurstCompile]
    public partial struct FollowToTargetJob : IJobEntity
    {
        [ReadOnly] public float DeltaTime;

        [BurstCompile]
        private void Execute(
            ref PhysicsVelocity physicsVelocity,
            ref LocalTransform localTransform,
            ref IsCloseDistance isCloseDistance,
            in MoveSpeed moveSpeed,
            in HorizontalRotationSpeed horizontalRotationSpeed,
            in FollowToTarget followToTarget,
            in FollowTarget followTarget)
        {
            float3 dir = followTarget.Position - localTransform.Position;
            float distanceSq = math.lengthsq(dir);
            float minDistanceSq = math.pow(followToTarget.CloseDistance, 2);
                
            if (distanceSq <= minDistanceSq)
            {
                isCloseDistance.Value = true;
                return;
            }
            
            isCloseDistance.Value = false;
            dir /= math.sqrt(distanceSq);
                
            localTransform.Rotation = math.slerp(
                localTransform.Rotation,
                quaternion.LookRotation(dir, math.up()),
                DeltaTime * horizontalRotationSpeed.Value);
                
            physicsVelocity.Linear = dir * moveSpeed.Value;
            physicsVelocity.Angular = 0;
        }
    }
    
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(UpdateFollowDataSystem))]
    public partial struct FollowToTargetSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<FollowToTarget>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var job = new FollowToTargetJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime
            };

            state.Dependency = job.ScheduleParallel(state.Dependency);
        }
    }
    
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct UpdateFollowDataSystem : ISystem
    {
        private ComponentLookup<LocalTransform> targetTransforms;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<FollowToTarget>();
            targetTransforms = state.GetComponentLookup<LocalTransform>(true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            targetTransforms.Update(ref state);
            var job = new UpdateFollowDataJob
            {
                TargetTransforms = targetTransforms
            };
            
            state.Dependency = job.ScheduleParallel(state.Dependency);
        }
    }
    
    [BurstCompile]
    public partial struct UpdateFollowDataJob : IJobEntity
    {
        [ReadOnly] public ComponentLookup<LocalTransform> TargetTransforms;
        
        [BurstCompile]
        private void Execute(ref FollowTarget followTarget, in FollowToTarget followToTarget)
        {
            followTarget.Position = TargetTransforms[followToTarget.Target].Position;
        }
    }
}