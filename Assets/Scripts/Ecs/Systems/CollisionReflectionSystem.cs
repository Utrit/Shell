using EntityComponentSystems.Components;
using Leopotam.EcsLite;
using UnityEngine;

namespace EntityComponentSystems.Systems
{
    public class CollisionReflectionSystem : IEcsRunSystem, IEcsInitSystem
    {
        private EcsWorld world;
        private EcsFilter filter;
        private EcsPool<TransformComponent> pool;
        private EcsPool<OnCollisionComponent> collisionPool;
        private EcsPool<UpdateNormalRequestComponent> normalPool;
        private EcsPool<SimpleMoveComponent> movePool;

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            filter = world.Filter<TransformComponent>().Inc<OnCollisionComponent>().Exc<PlayerComponent>().End();
            pool = world.GetPool<TransformComponent>();
            collisionPool = world.GetPool<OnCollisionComponent>();
            normalPool = world.GetPool<UpdateNormalRequestComponent>();
            movePool = world.GetPool<SimpleMoveComponent>();
        }
        
        public void Run(IEcsSystems systems)
        {
            
            foreach (var entity in filter)
            {
                ref TransformComponent transform = ref pool.Get(entity);
                ref OnCollisionComponent collision = ref collisionPool.Get(entity);
                ref SimpleMoveComponent move = ref movePool.Get(entity);
                move.moveDirection = Vector3.Reflect(transform.Direction, collision.collisionNormal);
                transform.Position = collision.collisionPoint;
                if(!normalPool.Has(entity))normalPool.Add(entity);
                collisionPool.Del(entity);
            }
        }
    }
}