using EntityComponenSystems.Components;
using Leopotam.EcsLite;
using UnityEngine;

namespace EntityComponenSystems.Systems
{
    public class MoveEntitySystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var filter = world.Filter<TransformComponent>().Inc<SimpleMoveComponent>().End();
            var pool = world.GetPool<TransformComponent>();
            var movepool = world.GetPool<SimpleMoveComponent>();
            foreach (var entity in filter)
            {
                ref TransformComponent transform = ref pool.Get(entity);
                ref SimpleMoveComponent move = ref movepool.Get(entity);
                transform.Position += transform.Direction * move.Speed * Time.deltaTime;
            }
        }
    }
}