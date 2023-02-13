using EntityComponentSystems.Components;
using EntityComponentSystems.Systems;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.Serialization;

namespace EntityComponentSystems
{
    public class EcsBootStrap : MonoBehaviour
{
    [SerializeField] private bool Debug;
    [SerializeField] private EntityView view;
    [SerializeField] private Transform playerTransform;
    private EcsWorld _world;
    private IEcsSystems _systems;
    private IEcsSystems _debugSystems;
    private EcsPool<TransformComponent> poolTransformCache;
    private EcsPool<RenderDataComponent> poolRenderCache;
    private EcsPool<UpdateNormalRequestComponent> poolRecalcNormal;
    private EcsPool<SimpleMoveComponent> poolSimpleMove;
    
    
    void Start () {
        _world = new EcsWorld ();
        
        _systems = new EcsSystems (_world);
        _systems
            .Add (new PlayerInputSystem())
            .Add (new PlayerMoveSystem())
            .Add (new CollisionSystem())
            .Add (new CollisionReflectionSystem())
            .Add (new UpdateNormalSystem())
            .Add (new MoveEntitySystem())
            .Add (new RenderCutoffSystem())
            .Add (new RenderSystem(view))
            .Add (new PlayerMonoSystem())
            .Init ();
        
        _debugSystems = new EcsSystems(_world);
        _debugSystems
            .Add(new DebugViewSystem())
            .Init();
        
        poolTransformCache = _world.GetPool<TransformComponent>();
        poolRenderCache = _world.GetPool<RenderDataComponent>();
        poolRecalcNormal = _world.GetPool<UpdateNormalRequestComponent>();
        poolSimpleMove = _world.GetPool<SimpleMoveComponent>();
        var playerEntity = _world.NewEntity();
        ref PlayerComponent playerComponent = ref _world.GetPool<PlayerComponent>().Add(playerEntity);
        playerComponent.speed = 5;
        _world.GetPool<PlayerInputComponent>().Add(playerEntity);
        ref TransformComponent playerTransformComponent = ref poolTransformCache.Add(playerEntity);
        ref SimpleMoveComponent simpleMoveComponent = ref poolSimpleMove.Add(playerEntity);
        playerTransformComponent.Position = playerTransform.position;
        playerTransformComponent.Direction = Vector3.zero;
        
        playerComponent.objectTransform = playerTransform;
        
        for (var i = 0; i < 10000; i++)
        {
            var entity = _world.NewEntity();
            
            ref TransformComponent transformComponent = ref poolTransformCache.Add(entity);
            transformComponent.Position = Random.insideUnitCircle;
            transformComponent.Direction = Random.insideUnitCircle;
            transformComponent.Size = 0.01f;
            ref SimpleMoveComponent simpleMove = ref poolSimpleMove.Add(entity);
            simpleMove.moveDirection = Random.insideUnitCircle*Random.Range(0.5f,2f);
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