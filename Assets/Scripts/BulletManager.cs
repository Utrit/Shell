using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class BulletManager : MonoBehaviour
{
    private const float BulletSize = 0.1f;
    private const int MaxBullets = 300000;
    private readonly Vector2 atlasSize = new Vector2(512, 512);
    private readonly Vector2 bulletCellSize = new Vector2(51, 51);
    private int atlasWight;

    private readonly Vector3[] vertices = new Vector3[MaxBullets * 4];
    private readonly Vector2[] uvs = new Vector2[MaxBullets * 4];
    private readonly int[] triangles = new int[MaxBullets*6];
    private int verticesPointer;
    private int trianglesPointer;
    private int uvsPointer;
    private Mesh bulletsMesh;
    private readonly List<Bullet> bullets = new List<Bullet>();

    private void Start()
    {
        atlasWight = (int)(atlasSize.x / bulletCellSize.x);

        bulletsMesh = new Mesh();
        bulletsMesh.RecalculateBounds(UnityEngine.Rendering.MeshUpdateFlags.DontRecalculateBounds);
        GetComponent<MeshFilter>().mesh = bulletsMesh;
        GenerateBulletTriangles();
    }

    private float _shift  = 0;

    public void FixedUpdate()
    {
        verticesPointer = 0;
        uvsPointer = 0;
        
        for (var i = 0; i < 10; i++)
        {
            _shift += 0.5f;
            Instantiate(new Bullet(Vector3.zero, Rotate2D(Vector3.right, _shift), 5, 0.1f, 0));
        }
        
        for (var i = bullets.Count-1; i > -1; i--)
        {
            var bullet = bullets[i];
            bullet.UpdateMove(Time.deltaTime);
            var raycast = Physics2D.Raycast(bullet.Position, bullet.Direction, bullet.Speed * Time.deltaTime);
            if (!(raycast.collider is null))
            {
                bullet.OnCollision(raycast, this);
            }
            GenerateBulletVertices(bullet);
            GenerateBulletUvs(bullet);
            if (bullet.Speed < 0.05f)
            {
                bullets.Remove(bullet);
            }
        }
        
        bulletsMesh.vertices = vertices;
        bulletsMesh.uv = uvs;
        bulletsMesh.SetTriangles(triangles, 0, verticesPointer + verticesPointer / 2, 0, false);
        bulletsMesh.bounds = new Bounds(Vector3.zero, Vector3.one * 5000f);
    }

    private void GenerateBulletVertices(Bullet bullet)
    {
        vertices[verticesPointer++] = bullet.Position - (bullet.Normal + bullet.Direction) * bullet.Size;
        vertices[verticesPointer++] = bullet.Position - (bullet.Normal - bullet.Direction) * bullet.Size;
        vertices[verticesPointer++] = bullet.Position + (bullet.Normal + bullet.Direction) * bullet.Size;
        vertices[verticesPointer++] = bullet.Position + (bullet.Normal - bullet.Direction) * bullet.Size;
    }

    private void GenerateBulletVertices(Vector3 pos, float angel)
    {
        vertices[verticesPointer++] = pos + Rotate2D(new Vector3(-BulletSize, -BulletSize, 0), angel);
        vertices[verticesPointer++] = pos + Rotate2D(new Vector3(-BulletSize, +BulletSize, 0), angel);
        vertices[verticesPointer++] = pos + Rotate2D(new Vector3(+BulletSize, +BulletSize, 0), angel);
        vertices[verticesPointer++] = pos + Rotate2D(new Vector3(+BulletSize, -BulletSize, 0), angel);
    }

    private void GenerateBulletUvs(Bullet bullet)
    {
        var atlasX = bullet.AtlasIndex % atlasWight;
        var atlasY = bullet.AtlasIndex / atlasWight;

        uvs[uvsPointer++] = new Vector2(bulletCellSize.x * (atlasX + 0) / atlasSize.x, bulletCellSize.y * (atlasY + 0) / atlasSize.y);
        uvs[uvsPointer++] = new Vector2(bulletCellSize.x * (atlasX + 0) / atlasSize.x, bulletCellSize.y * (atlasY + 1) / atlasSize.y);
        uvs[uvsPointer++] = new Vector2(bulletCellSize.x * (atlasX + 1) / atlasSize.x, bulletCellSize.y * (atlasY + 1) / atlasSize.y);
        uvs[uvsPointer++] = new Vector2(bulletCellSize.x * (atlasX + 1) / atlasSize.x, bulletCellSize.y * (atlasY + 0) / atlasSize.y);
        
    }

    private Vector3 Rotate2D(Vector3 vector, float angel)
    {
        var cos = Mathf.Cos(angel);
        var sin = Mathf.Sin(angel);
        var bufferX = vector.x * cos - vector.y * sin;
        vector.y = vector.x * sin + vector.y * cos;
        vector.x = bufferX;
        return vector;
    }

    private void GenerateBulletTriangles()
    {
        for (var index = 0; index < MaxBullets * 4; index+=4)
        {
            triangles[trianglesPointer++] = index;
            triangles[trianglesPointer++] = index + 1;
            triangles[trianglesPointer++] = index + 2;
            triangles[trianglesPointer++] = index;
            triangles[trianglesPointer++] = index + 2;
            triangles[trianglesPointer++] = index + 3;
        }
    }

    public bool Instantiate(Bullet bullet)
    {
        if (bullets.Count >= MaxBullets) return false;
        bullets.Add(bullet);
        return true;
    }

    public void Destroy(Bullet bullet)
    {
        bullets.Remove(bullet);
    }
}
