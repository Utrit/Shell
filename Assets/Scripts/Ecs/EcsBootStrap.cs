using System.Collections;
using System.Collections.Generic;
using Leopotam.EcsLite;
using UnityEngine;

public class EcsBootStrap : MonoBehaviour
{
    [SerializeField] private bool Debug;
    EcsWorld _world;
    IEcsSystems _systems;
    IEcsSystems _debugSystems;

    void Start () {
        _world = new EcsWorld ();
        
        _systems = new EcsSystems (_world);
        _systems
            .Add (new MoveEntitySystem())
            .Init ();
        
        _debugSystems = new EcsSystems(_world);
        _debugSystems
            .Add(new DebugViewSystem())
            .Init();
        
        var pool = _world.GetPool<TransformComponent>();
        for (int i = 0; i < 100000; i++)
        {
            int entiy = _world.NewEntity();
            pool.Add(entiy);
            ref TransformComponent transform = ref pool.Get(entiy);
            transform.Position = Random.insideUnitCircle;
            transform.Speed = 1;
            transform.Direction = Random.insideUnitCircle;
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

struct RenderDataComponent
{
    public float Size;
    public int AtlasIndex;
}

struct TransformComponent
{
    public Vector3 Position;
    public Vector3 Direction;
    public Vector3 Normal;
    public float Speed;
}
