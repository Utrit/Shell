using EntityComponentSystems.Components;
using Leopotam.EcsLite;
using UnityEngine;

namespace EntityComponentSystems.Systems
{
    public class PlayerMonoSystem : IEcsRunSystem, IEcsInitSystem
    {
        private EcsWorld world;
        private EcsFilter filter;

        private EcsPool<TransformComponent> transformPool;

        private EcsPool<PlayerComponent> playerPool;
        
        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            filter = world.Filter<PlayerComponent>().Inc<PlayerInputComponent>().Inc<TransformComponent>().End();
            transformPool = world.GetPool<TransformComponent>();
            playerPool = world.GetPool<PlayerComponent>();
        }
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in filter)
            {
                ref TransformComponent transformComponent = ref transformPool.Get(entity);
                ref PlayerComponent playerComponent = ref playerPool.Get(entity);
                playerComponent.objectTransform.position = transformComponent.Position;
                playerComponent.objectTransform.rotation = Quaternion.LookRotation(Vector3.forward,transformComponent.Direction);
            }
        }
    }
}