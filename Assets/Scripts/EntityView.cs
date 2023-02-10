using EntityComponenSystems.Components;
using Leopotam.EcsLite;
using UnityEngine;


[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class EntityView : MonoBehaviour
{
    private const float EntitySize = 0.1f;
    private const int MaxEntity = 300000;
    private readonly Vector2 atlasSize = new Vector2(512, 512);
    private readonly Vector2 entityAtlasCellSize = new Vector2(51, 51);
    private int atlasWight;

    private readonly Vector3[] vertices = new Vector3[MaxEntity * 4];
    private readonly Vector2[] uvs = new Vector2[MaxEntity * 4];
    private readonly int[] triangles = new int[MaxEntity*6];
    private int verticesPointer;
    private int trianglesPointer;
    private int uvsPointer;
    private Mesh viewMesh;

    private void Start()
    {
        atlasWight = (int)(atlasSize.x / entityAtlasCellSize.x);
        
        viewMesh = new Mesh();
        viewMesh.RecalculateBounds(UnityEngine.Rendering.MeshUpdateFlags.DontRecalculateBounds);
        GetComponent<MeshFilter>().mesh = viewMesh;
        GenerateEntityTriangles();
    }

    private readonly Bounds boundOverride = new Bounds(Vector3.zero, Vector3.one * 5000f);
    public void RenderObjects(EcsFilter filter,EcsWorld world)
    {
        verticesPointer = 0;
        uvsPointer = 0;
        var renderPool = world.GetPool<RenderDataComponent>();
        var transformPool = world.GetPool<TransformComponent>();
        foreach (var entity in filter)
        {
            ref RenderDataComponent renderData = ref renderPool.Get(entity);
            ref TransformComponent transformComponent = ref transformPool.Get(entity);
            GenerateEntityVertices(transformComponent);
            GenerateEntityUvs(transformComponent,renderData);
        }

        viewMesh.vertices = vertices;
        viewMesh.uv = uvs;
        viewMesh.SetTriangles(triangles, 0, verticesPointer + verticesPointer / 2, 0, false);
        viewMesh.bounds = boundOverride;
    }

    private void GenerateEntityVertices(TransformComponent entityObject)
    {
        vertices[verticesPointer++] = entityObject.Position - (entityObject.Normal + entityObject.Direction) * entityObject.Size;
        vertices[verticesPointer++] = entityObject.Position - (entityObject.Normal - entityObject.Direction) * entityObject.Size;
        vertices[verticesPointer++] = entityObject.Position + (entityObject.Normal + entityObject.Direction) * entityObject.Size;
        vertices[verticesPointer++] = entityObject.Position + (entityObject.Normal - entityObject.Direction) * entityObject.Size;
    }

    private void GenerateEntityVertices(Vector3 pos, float angel)
    {
        vertices[verticesPointer++] = pos + Rotate2D(new Vector3(-EntitySize, -EntitySize, 0), angel);
        vertices[verticesPointer++] = pos + Rotate2D(new Vector3(-EntitySize, +EntitySize, 0), angel);
        vertices[verticesPointer++] = pos + Rotate2D(new Vector3(+EntitySize, +EntitySize, 0), angel);
        vertices[verticesPointer++] = pos + Rotate2D(new Vector3(+EntitySize, -EntitySize, 0), angel);
    }

    private void GenerateEntityUvs(TransformComponent entityObject,RenderDataComponent renderData)
    {
        var atlasX = renderData.AtlasIndex % atlasWight;
        var atlasY = renderData.AtlasIndex / atlasWight;

        uvs[uvsPointer++] = new Vector2(entityAtlasCellSize.x * (atlasX + 0) / atlasSize.x, entityAtlasCellSize.y * (atlasY + 0) / atlasSize.y);
        uvs[uvsPointer++] = new Vector2(entityAtlasCellSize.x * (atlasX + 0) / atlasSize.x, entityAtlasCellSize.y * (atlasY + 1) / atlasSize.y);
        uvs[uvsPointer++] = new Vector2(entityAtlasCellSize.x * (atlasX + 1) / atlasSize.x, entityAtlasCellSize.y * (atlasY + 1) / atlasSize.y);
        uvs[uvsPointer++] = new Vector2(entityAtlasCellSize.x * (atlasX + 1) / atlasSize.x, entityAtlasCellSize.y * (atlasY + 0) / atlasSize.y);
        
    }

    private static Vector3 Rotate2D(Vector3 vector, float angel)
    {
        var cos = Mathf.Cos(angel);
        var sin = Mathf.Sin(angel);
        var bufferX = vector.x * cos - vector.y * sin;
        vector.y = vector.x * sin + vector.y * cos;
        vector.x = bufferX;
        return vector;
    }

    private void GenerateEntityTriangles()
    {
        for (var index = 0; index < MaxEntity * 4; index+=4)
        {
            triangles[trianglesPointer++] = index;
            triangles[trianglesPointer++] = index + 1;
            triangles[trianglesPointer++] = index + 2;
            triangles[trianglesPointer++] = index;
            triangles[trianglesPointer++] = index + 2;
            triangles[trianglesPointer++] = index + 3;
        }
    }
}
