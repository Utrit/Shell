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
        
        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            filter = world.Filter<TransformComponent>().Inc<OnCollisionComponent>().End();
            pool = world.GetPool<TransformComponent>();
            collisionPool = world.GetPool<OnCollisionComponent>();
            normalPool = world.GetPool<UpdateNormalRequestComponent>();
        }
        
        public void Run(IEcsSystems systems)
        {
            
            foreach (var entity in filter)
            {
                ref TransformComponent transform = ref pool.Get(entity);
                ref OnCollisionComponent collision = ref collisionPool.Get(entity);
                transform.Direction = Vector3.Reflect(transform.Direction, collision.collisionNormal);
                transform.Position = collision.collisionPoint;
                if(!normalPool.Has(entity))normalPool.Add(entity);
                collisionPool.Del(entity);
            }
        }
    }
}