using EntityComponentSystems.Components;
using Leopotam.EcsLite;

namespace EntityComponentSystems.Systems
{
    public class RenderSystem : IEcsRunSystem, IEcsInitSystem
    {
        private EntityView view;
        private EcsWorld world;
        private EcsFilter filter;
        private EcsPool<InViewComponent> poolInView;
        
        public RenderSystem(EntityView view)
        {
            this.view = view;
        }
        
        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            filter = world.Filter<RenderDataComponent>().Inc<TransformComponent>().Inc<InViewComponent>().End();
            poolInView = world.GetPool<InViewComponent>();
        }
        
        public void Run(IEcsSystems systems)
        {
            view.RenderObjects(filter,world);
            foreach (var entity in filter)
            {
                poolInView.Del(entity);
            }
        }
    }
}