using EntityComponenSystems.Components;
using Leopotam.EcsLite;

namespace EntityComponenSystems.Systems
{
    public class RenderSystem : IEcsRunSystem
    {
        private EntityView view;
        public RenderSystem(EntityView view)
        {
            this.view = view;
        }
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var filter = world.Filter<RenderDataComponent>().Inc<TransformComponent>().Inc<InViewComponent>().End();
            var poolInView = world.GetPool<InViewComponent>();
            view.RenderObjects(filter,world);
            foreach (var entity in filter)
            {
                poolInView.Del(entity);
            }
        }
    }
}