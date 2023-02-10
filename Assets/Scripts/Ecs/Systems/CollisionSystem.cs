using EntityComponenSystems.Components;
using Leopotam.EcsLite;
using UnityEngine;

namespace EntityComponenSystems.Systems
{
    public class CollisionSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var filter = world.Filter<TransformComponent>().Inc<SimpleMoveComponent>().End();
            var pool = world.GetPool<TransformComponent>();
            var collisionPool = world.GetPool<OnCollisionComponent>();
            var movePool = world.GetPool<SimpleMoveComponent>();
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