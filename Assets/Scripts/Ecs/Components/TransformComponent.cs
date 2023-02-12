using UnityEngine;

namespace EntityComponentSystems.Components
{
    struct TransformComponent
    {
        public Vector3 Position;
        public Vector3 Direction;
        public Vector3 Normal;
        public float Size;
    }
}