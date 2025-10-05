using Unity.Entities;
using UnityEngine;

namespace TestRPG.ECS
{
    public class PickAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject pickObject;
        
        public class Baker : Baker<PickAuthoring>
        {
            public override void Bake(PickAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                Entity pickEntity = authoring.pickObject != null
                    ? GetEntity(authoring.pickObject, TransformUsageFlags.Dynamic)
                    : Entity.Null;
                
                AddComponent(entity, new Pick
                {
                    PickEntity = pickEntity
                });
            }
        }
    }

    public struct Pick : IComponentData
    {
        public Entity PickEntity;
    }
}