using EntityComponentSystems.Components;
using Extensions;
using Leopotam.EcsLite;
using UnityEngine;

namespace EntityComponentSystems.Systems
{
    public class PlayerMoveSystem : IEcsRunSystem, IEcsInitSystem
    {
        private EcsWorld world;
        private EcsFilter filter;
        private EcsPool<PlayerInputComponent> pool;
        private EcsPool<TransformComponent> transformPool;
        private EcsPool<UpdateNormalRequestComponent> normalPool;
        private EcsPool<SimpleMoveComponent> movePool;
        private EcsPool<PlayerComponent> playerPool;

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            filter = world.Filter<PlayerComponent>().Inc<PlayerInputComponent>().Inc<TransformComponent>().End();
            pool = world.GetPool<PlayerInputComponent>();
            transformPool = world.GetPool<TransformComponent>();
            normalPool = world.GetPool<UpdateNormalRequestComponent>();
            movePool = world.GetPool<SimpleMoveComponent>();
            playerPool = world.GetPool<PlayerComponent>();
        }
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in filter)
            {
                ref PlayerInputComponent playerInputComponent = ref pool.Get(entity);
                ref TransformComponent transformComponent = ref transformPool.Get(entity);
                ref SimpleMoveComponent moveComponent = ref movePool.Get(entity);
                ref PlayerComponent playerComponent = ref playerPool.Get(entity);
                if(!normalPool.Has(entity))normalPool.Add(entity);
                transformComponent.Direction = playerInputComponent.mouseDirection;
                var moveDirecton = playerInputComponent.mouseDirection * -playerInputComponent.moveDirection.y;
                moveDirecton += ExtensionTool.Get2DNormal(playerInputComponent.mouseDirection) * -playerInputComponent.moveDirection.x;
                moveComponent.moveDirection +=  moveDirecton * playerComponent.speed * Time.deltaTime;
                moveComponent.moveDirection -= moveComponent.moveDirection * 1f * Time.deltaTime;
            }
        }
    }
}