using UnityEngine;

public class Bullet
{
    private Vector3 direction;

    public Vector3 Position { get; private set; }

    public Vector3 Direction => direction;
    public Vector3 Normal { get; private set; }

    public float Speed { get; private set; }

    public float Size { get; }

    public int AtlasIndex { get; }

    public Bullet(Vector3 position, Vector3 direction, float speed, float size,int atlasIndex)
    {
        this.Position = position;
        this.direction = direction.normalized;
        Normal = new Vector3(this.direction.y, -this.direction.x);
        this.Speed = speed;
        this.Size = size;
        this.AtlasIndex = atlasIndex;
    }

    public void UpdateMove(float deltaTime)
    {
        Position += direction * (Speed * deltaTime);
        Speed -= Speed * 0.3f * deltaTime;
        
    }

    public void OnCollision(RaycastHit2D hit,BulletManager manager)
    {
        direction = Vector3.Reflect(direction, hit.normal);
        Position = hit.point;
        Normal = new Vector3(direction.y, -direction.x);
    }
}
