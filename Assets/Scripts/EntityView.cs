using System.Collections.Generic;
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
    private readonly List<EntityObject> entityArray = new List<EntityObject>();

    private void Start()
    {
        atlasWight = (int)(atlasSize.x / entityAtlasCellSize.x);

        viewMesh = new Mesh();
        viewMesh.RecalculateBounds(UnityEngine.Rendering.MeshUpdateFlags.DontRecalculateBounds);
        GetComponent<MeshFilter>().mesh = viewMesh;
        GenerateEntityTriangles();
    }

    private float _shift  = 0;

    public void FixedUpdate()
    {
        verticesPointer = 0;
        uvsPointer = 0;
        
        for (var i = 0; i < 3; i++)
        {
            _shift += 0.5f;
            Instantiate(new EntityObject(Vector3.zero, Rotate2D(Vector3.right, _shift), 5, 0.1f, Random.Range(0,20)));
        }
        
        for (var i = entityArray.Count-1; i > -1; i--)
        {
            var entity = entityArray[i];
            entity.UpdateMove(Time.deltaTime);
            var raycast = Physics2D.Raycast(entity.Position, entity.Direction, entity.Speed * Time.deltaTime);
            if (raycast.collider)
            {
                entity.OnCollision(raycast, this);
            }
            GenerateEntityVertices(entity);
            GenerateEntityUvs(entity);
            if (entity.Speed < 0.1f)
            {
                entityArray.Remove(entity);
            }
        }
        
        viewMesh.vertices = vertices;
        viewMesh.uv = uvs;
        viewMesh.SetTriangles(triangles, 0, verticesPointer + verticesPointer / 2, 0, false);
        viewMesh.bounds = new Bounds(Vector3.zero, Vector3.one * 5000f);
    }

    private void GenerateEntityVertices(EntityObject entityObject)
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

    private void GenerateEntityUvs(EntityObject entityObject)
    {
        var atlasX = entityObject.AtlasIndex % atlasWight;
        var atlasY = entityObject.AtlasIndex / atlasWight;

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

    public bool Instantiate(EntityObject entityObject)
    {
        if (entityArray.Count >= MaxEntity) return false;
        entityArray.Add(entityObject);
        return true;
    }

    public void Destroy(EntityObject entityObject)
    {
        entityArray.Remove(entityObject);
    }
}
