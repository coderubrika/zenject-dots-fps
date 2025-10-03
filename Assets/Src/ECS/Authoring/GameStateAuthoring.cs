using Unity.Entities;
using UnityEngine;

namespace TestRPG.ECS
{
    public class GameStateAuthoring : MonoBehaviour
    {
        public class Baker : Baker<GameStateAuthoring>
        {
            public override void Bake(GameStateAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new GameState());
            }
        }
    }

    public struct GameState : IComponentData
    {
        public GameStateVariant State;
    }

    public enum GameStateVariant
    {
        Play,
        Stop,
        Pause,
        Resume
    }
}