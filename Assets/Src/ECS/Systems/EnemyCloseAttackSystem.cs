using Unity.Burst;
using Unity.Entities;

namespace TestRPG.ECS
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct EnemyCloseAttackSystem : ISystem
    {
        private ComponentLookup<Health> healths;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<IsCloseDistance>();
            state.RequireForUpdate<Enemy>();
            state.RequireForUpdate<RepeatingAttackSettings>();
            state.RequireForUpdate<AttackTarget>();
            state.RequireForUpdate<Damage>();

            healths = state.GetComponentLookup<Health>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            healths.Update(ref state);

            var canAttackJob = new EnemyCanAttackByCloseJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime
            };
            state.Dependency = canAttackJob.ScheduleParallel(state.Dependency);
            
            var attackJob = new EnemyCloseAttackJob
            {
                Healths = healths
            };
            state.Dependency = attackJob.Schedule(state.Dependency);
        }
    }
    
    [BurstCompile]
    [WithAll(typeof(Enemy))]
    public partial struct EnemyCloseAttackJob : IJobEntity
    {
        public ComponentLookup<Health> Healths;
        
        [BurstCompile]
        private void Execute(
            ref CanAttack canAttack,
            in AttackTarget attackTarget,
            in Damage damage)
        {
            if (!canAttack.Value)
                goto End;
            
            Health targetHealth = Healths[attackTarget.Target];
            targetHealth.Value -= damage.Value;
            Healths[attackTarget.Target] = targetHealth;
            canAttack.Value = false;
            
            End: {}
        }
    }
    
    [BurstCompile]
    [WithAll(typeof(Enemy))]
    public partial struct EnemyCanAttackByCloseJob : IJobEntity
    {
        public float DeltaTime;
        
        [BurstCompile]
        private void Execute(
            in IsCloseDistance isCloseDistance,
            ref RepeatingAttackSettings repeatingAttackSettings,
            ref CanAttack canAttack)
        {
            if (canAttack.Value)
                goto End;
            
            if (!isCloseDistance.Value)
            {
                repeatingAttackSettings.CurrentTimeSec = 0;
                goto End;
            }
            
            repeatingAttackSettings.CurrentTimeSec += DeltaTime;

            if (repeatingAttackSettings.CurrentTimeSec >= repeatingAttackSettings.TimeRateSec)
            {
                repeatingAttackSettings.CurrentTimeSec = 0;
                canAttack.Value = true;
            }
            
            End: {}
        }
    }
}