using EntityComponentSystems.Components;
using Leopotam.EcsLite;
using UnityEngine;

namespace EntityComponentSystems.Systems
{
    public class RenderCutoffSystem : IEcsRunSystem,IEcsInitSystem
    {
        private Vector3 cameraSizeCache;
        private int screenRatio;
        private Vector3 CameraSize
        {
            get
            {
                if (Screen.width + Screen.height != screenRatio)
                {
                    var cameraPosition = Camera.main.transform.position;
                    cameraSizeCache = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height))-cameraPosition;
                    screenRatio = Screen.width + Screen.height;
                    return cameraSizeCache;
                }
                return cameraSizeCache;
            }
        }
        private EcsWorld world;
        private EcsFilter filter;
        private EcsPool<InViewComponent> poolInView;
        private EcsPool<TransformComponent> poolTransform;
        private Bounds cameraBounds;
        
        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            filter = world.Filter<RenderDataComponent>().Inc<TransformComponent>().End();
            poolInView = world.GetPool<InViewComponent>();
            poolTransform = world.GetPool<TransformComponent>();
            cameraBounds = new Bounds();
        }
        
        public void Run(IEcsSystems systems)
        {
            
            var cameraPosition = Camera.main.transform.position;
            cameraPosition.z = 0;
            cameraBounds.center = cameraPosition;
            cameraBounds.size = CameraSize * 2;
            foreach (var entity in filter)
            {
                ref TransformComponent transformComponent  = ref poolTransform.Get(entity);
                if (cameraBounds.Contains(transformComponent.Position))
                {
                    poolInView.Add(entity);
                }
            }
        }
    }
}