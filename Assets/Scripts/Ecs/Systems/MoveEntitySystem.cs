using EntityComponentSystems.Components;
using Leopotam.EcsLite;
using UnityEngine;

namespace EntityComponentSystems.Systems
{
    public class MoveEntitySystem : IEcsRunSystem, IEcsInitSystem
    {
        private EcsWorld world;
        private EcsFilter filter;
        private EcsPool<TransformComponent> pool;
        private EcsPool<SimpleMoveComponent> movePool;
        
        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            filter = world.Filter<TransformComponent>().Inc<SimpleMoveComponent>().End();
            pool = world.GetPool<TransformComponent>();
            movePool = world.GetPool<SimpleMoveComponent>();
        }
        
        public void Run(IEcsSystems systems)
        {

            foreach (var entity in filter)
            {
                ref TransformComponent transform = ref pool.Get(entity);
                ref SimpleMoveComponent move = ref movePool.Get(entity);
                transform.Position += transform.Direction * move.Speed * Time.deltaTime;
            }
        }
    }
}