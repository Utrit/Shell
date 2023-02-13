using UnityEngine;

namespace Extensions
{
    public static class ExtensionTool
    {
        public static Vector3 Get2DNormal(Vector3 vector)
        {
           return new Vector3(vector.y,-vector.x).normalized;
        }
    }
}