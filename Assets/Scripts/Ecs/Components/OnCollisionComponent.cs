using UnityEngine;

namespace EntityComponentSystems.Components
{
    struct OnCollisionComponent
    {
        public Vector3 collisionPoint;
        public Vector3 collisionNormal;
    }
}