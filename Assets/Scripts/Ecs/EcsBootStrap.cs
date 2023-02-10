using EntityComponenSystems.Components;
using EntityComponenSystems.Systems;
using Leopotam.EcsLite;
using UnityEngine;

namespace EntityComponenSystems
{
    public class EcsBootStrap : MonoBehaviour
{
    [SerializeField] private bool Debug;
    [SerializeField] private EntityView view;
    EcsWorld _world;
    IEcsSystems _systems;
    IEcsSystems _debugSystems;

    void Start () {
        _world = new EcsWorld ();
        
        _systems = new EcsSystems (_world);
        _systems
            .Add (new UpdateNormalSystem())
            .Add (new MoveEntitySystem())
            .Add (new RenderCutoffSystem())
            .Add (new RenderSystem(view))
            .Init ();
        
        _debugSystems = new EcsSystems(_world);
        _debugSystems
            .Add(new DebugViewSystem())
            .Init();
        
        var poolTransformCache = _world.GetPool<TransformComponent>();
        var poolRenderCache = _world.GetPool<RenderDataComponent>();
        var poolRecalcNormal = _world.GetPool<UpdateNormalRequestComponent>();
        var poolSimpleMove = _world.GetPool<SimpleMoveComponent>();
        for (var i = 0; i < 50000; i++)
        {
            var entity = _world.NewEntity();
            
            ref TransformComponent transformComponent = ref poolTransformCache.Add(entity);
            transformComponent.Position = Random.insideUnitCircle;
            transformComponent.Direction = Random.insideUnitCircle;
            transformComponent.Size = 0.01f;
            ref SimpleMoveComponent simpleMove = ref poolSimpleMove.Add(entity);
            simpleMove.Speed = 0.03f;
            ref RenderDataComponent renderData = ref poolRenderCache.Add(entity);
            renderData.AtlasIndex = Random.Range(0,100);
            poolRecalcNormal.Add(entity);
        }
    }
    
    void Update () {
        _systems?.Run ();
        if(Debug) _debugSystems?.Run();
    }

    void OnDestroy () {
        if (_systems != null) {
            _systems.Destroy ();
            _systems = null;
        }
        if (_world != null) {
            _world.Destroy ();
            _world = null;
        }
    }
}
}

namespace EntityComponenSystems.Components
{
}