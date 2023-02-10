using EntityComponenSystems.Components;
using Leopotam.EcsLite;
using UnityEngine;

namespace EntityComponenSystems.Systems
{
    public class RenderCutoffSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var filter = world.Filter<RenderDataComponent>().Inc<TransformComponent>().End();
            var poolInView = world.GetPool<InViewComponent>();
            var poolTransform = world.GetPool<TransformComponent>();
            var cameraPosition = Camera.main.transform.position;
            cameraPosition.z = 0;
            var bounds = new Bounds(cameraPosition, Vector3.one * Camera.main.orthographicSize * 3);
            foreach (var entity in filter)
            {
                ref TransformComponent transformComponent  = ref poolTransform.Get(entity);
                if (bounds.Contains(transformComponent.Position))
                {
                    poolInView.Add(entity);
                }
            }
        }
    }
}