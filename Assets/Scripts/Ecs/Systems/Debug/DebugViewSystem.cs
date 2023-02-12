using EntityComponentSystems.Components;
using Leopotam.EcsLite;
using UnityEngine;

namespace EntityComponentSystems.Systems
{
    public class DebugViewSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var filter = world.Filter<TransformComponent>().End();
            var pool = world.GetPool<TransformComponent>();
        
            foreach (var entity in filter)
            {
                ref TransformComponent transform = ref pool.Get(entity);
                Debug.DrawLine(transform.Position,transform.Position+Vector3.forward);
            }
        }
    }
}