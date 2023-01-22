using UnityEngine;

public class EntityObject
{

    public Vector3 Position { get; private set; }

    public Vector3 Direction { get; private set; }
    public Vector3 Normal { get; private set; }

    public float Speed { get; private set; }

    public float Size { get; }

    public int AtlasIndex { get; }

    public EntityObject(Vector3 position, Vector3 direction, float speed, float size,int atlasIndex)
    {
        this.Position = position;
        this.Direction = direction.normalized;
        Normal = new Vector3(this.Direction.y, -this.Direction.x);
        this.Speed = speed;
        this.Size = size;
        this.AtlasIndex = atlasIndex;
    }

    public void UpdateMove(float deltaTime)
    {
        Position += Direction * (Speed * deltaTime);
        Speed -= Speed * 0.3f * deltaTime;
        
    }

    public void OnCollision(RaycastHit2D hit,EntityView manager)
    {
        Direction = Vector3.Reflect(Direction, hit.normal);
        Position = hit.point;
        Normal = new Vector3(Direction.y, -Direction.x);
        Speed *= 0.4f;
    }
}
