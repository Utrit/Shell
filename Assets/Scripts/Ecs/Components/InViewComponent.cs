﻿using UnityEngine;

namespace EntityComponenSystems.Components
{
    struct InViewComponent
    {
    
    }

    struct OnCollisionComponent
    {
        public Vector3 collisionPoint;
        public Vector3 collisionNormal;
    }
}