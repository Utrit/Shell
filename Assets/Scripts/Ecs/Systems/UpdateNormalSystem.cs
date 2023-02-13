using EntityComponentSystems.Components;
using Leopotam.EcsLite;
using Extensions;

namespace EntityComponentSystems.Systems
{
    public class UpdateNormalSystem : IEcsRunSystem, IEcsInitSystem
    {
        EcsWorld world;
        EcsFilter filter;
        EcsPool<TransformComponent> pool;
        EcsPool<UpdateNormalRequestComponent> requestPool;
        
        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            filter = world.Filter<UpdateNormalRequestComponent>().Inc<TransformComponent>().End();
            pool = world.GetPool<TransformComponent>();
            requestPool = world.GetPool<UpdateNormalRequestComponent>();
        }
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in filter)
            {
                ref TransformComponent transform = ref pool.Get(entity);
                transform.Direction = transform.Direction.normalized;
                transform.Normal = ExtensionTool.Get2DNormal(transform.Direction).normalized;
                requestPool.Del(entity);
            }
        }
    }
}