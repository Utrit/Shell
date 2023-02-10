using EntityComponenSystems.Components;
using Leopotam.EcsLite;
using UnityEngine;

namespace EntityComponenSystems.Systems
{
    public class CollisionReflectionSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var filter = world.Filter<TransformComponent>().Inc<OnCollisionComponent>().End();
            var pool = world.GetPool<TransformComponent>();
            var collisionPool = world.GetPool<OnCollisionComponent>();
            var normalPool = world.GetPool<UpdateNormalRequestComponent>();
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