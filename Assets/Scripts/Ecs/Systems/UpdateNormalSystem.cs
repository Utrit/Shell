using EntityComponenSystems.Components;
using Leopotam.EcsLite;
using UnityEngine;

namespace EntityComponenSystems.Systems
{
    public class UpdateNormalSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var filter = world.Filter<UpdateNormalRequestComponent>().Inc<TransformComponent>().End();
            var pool = world.GetPool<TransformComponent>();
            var requestPool = world.GetPool<UpdateNormalRequestComponent>();
            foreach (var entity in filter)
            {
                ref TransformComponent transform = ref pool.Get(entity);
                transform.Direction = transform.Direction.normalized;
                transform.Normal = new Vector3(transform.Direction.y,-transform.Direction.x).normalized;
                requestPool.Del(entity);
            }
        }
    }
}