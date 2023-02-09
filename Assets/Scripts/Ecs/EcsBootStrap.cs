using Leopotam.EcsLite;
using UnityEngine;

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
            .Add (new RenderSystem(view))
            .Init ();
        
        _debugSystems = new EcsSystems(_world);
        _debugSystems
            .Add(new DebugViewSystem())
            .Init();
        
        var poolTransformCache = _world.GetPool<TransformComponent>();
        var poolRenderCache = _world.GetPool<RenderDataComponent>();
        var poolRecalcNormal = _world.GetPool<UpdateNormalRequest>();
        
        for (var i = 0; i < 10000; i++)
        {
            var entity = _world.NewEntity();
            
            ref TransformComponent transformComponent = ref poolTransformCache.Add(entity);
            transformComponent.Position = Random.insideUnitCircle;
            transformComponent.Speed = 1;
            transformComponent.Direction = Random.insideUnitCircle;
            transformComponent.Size = 0.1f;
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
        var filter = world.Filter<RenderDataComponent>().Inc<TransformComponent>().End();
        view.RenderObjects(filter,world);
    }
}

public class UpdateNormalSystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        var world = systems.GetWorld();
        var filter = world.Filter<UpdateNormalRequest>().Inc<TransformComponent>().End();
        var pool = world.GetPool<TransformComponent>();
        var requestPool = world.GetPool<UpdateNormalRequest>();
        foreach (var entity in filter)
        {
            ref TransformComponent transform = ref pool.Get(entity);
            transform.Direction = transform.Direction.normalized;
            transform.Normal = new Vector3(transform.Direction.y,-transform.Direction.x).normalized;
            requestPool.Del(entity);
        }
    }
}

public class MoveEntitySystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        var world = systems.GetWorld();
        var filter = world.Filter<TransformComponent>().End();
        var pool = world.GetPool<TransformComponent>();
        
        foreach (var entity in filter)
        {
            ref TransformComponent transform = ref pool.Get(entity);
            transform.Position += transform.Direction * transform.Speed * Time.deltaTime;
        }
    }
}

public class DebugViewSystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        var world = systems.GetWorld();
        var filter = world.Filter<TransformComponent>().End();
        var pool = world.GetPool<TransformComponent>();
        
        foreach (var entity in filter)
        {
            ref TransformComponent transform = ref pool.Get(entity);
            Debug.DrawLine(transform.Position,transform.Position+Vector3.forward);
        }
    }
}

struct UpdateNormalRequest
{
}

struct RenderDataComponent
{
    public int AtlasIndex;
}

struct TransformComponent
{
    public Vector3 Position;
    public Vector3 Direction;
    public Vector3 Normal;
    public float Speed;
    public float Size;
}
