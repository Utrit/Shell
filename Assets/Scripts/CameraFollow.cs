using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform followingTransform;

        private void Start()
        {
            if (!followingTransform)
            {
                Debug.LogError("Following transform not assigned");
            }
        }

        private void Update()
        {
            var followPosition = followingTransform.position;
            followPosition.z = -10;
            transform.position = Vector3.Lerp(transform.position, followPosition, Time.deltaTime);
        }
    }
}