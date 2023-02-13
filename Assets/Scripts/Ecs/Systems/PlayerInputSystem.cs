using EntityComponentSystems.Components;
using Leopotam.EcsLite;
using UnityEngine;

namespace EntityComponentSystems.Systems
{
    public class PlayerInputSystem : IEcsRunSystem, IEcsInitSystem
    {
        private EcsWorld world;
        private EcsFilter filter;
        private EcsPool<PlayerInputComponent> pool;
        private EcsPool<TransformComponent> transformPool;

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            filter = world.Filter<PlayerComponent>().Inc<PlayerInputComponent>().Inc<TransformComponent>().End();
            pool = world.GetPool<PlayerInputComponent>();
            transformPool = world.GetPool<TransformComponent>();
        }
        
        public void Run(IEcsSystems systems)
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            foreach (var entity in filter)
            {
                ref PlayerInputComponent playerInputComponent = ref pool.Get(entity);
                ref TransformComponent transformComponent = ref transformPool.Get(entity);
                playerInputComponent.moveDirection = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
                var mouseVector = transformComponent.Position - mousePos;
                mouseVector.z = 0;
                playerInputComponent.mouseDirection = mouseVector.normalized;
            }
        }
    }
}