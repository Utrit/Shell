using EntityComponentSystems.Components;
using Leopotam.EcsLite;
using UnityEngine;

namespace EntityComponentSystems.Systems
{
    public class CollisionSystem : IEcsRunSystem, IEcsInitSystem
    {
        private EcsWorld world;
        private EcsFilter filter;
        private EcsPool<TransformComponent> pool;
        private EcsPool<OnCollisionComponent> collisionPool;
        private EcsPool<SimpleMoveComponent> movePool;
        
        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            filter = world.Filter<TransformComponent>().Inc<SimpleMoveComponent>().End();
            pool = world.GetPool<TransformComponent>();
            collisionPool = world.GetPool<OnCollisionComponent>();
            movePool = world.GetPool<SimpleMoveComponent>();
        }
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in filter)
            {
                ref TransformComponent transform = ref pool.Get(entity);
                ref SimpleMoveComponent move = ref movePool.Get(entity);
                var raycast = Physics2D.Raycast(transform.Position, transform.Direction, move.Speed * Time.deltaTime);
                if (raycast.collider)
                {
                    ref OnCollisionComponent collision = ref collisionPool.Add(entity);
                    collision.collisionNormal = raycast.normal;
                    collision.collisionPoint = raycast.point;
                }
            }
        }
    }
}