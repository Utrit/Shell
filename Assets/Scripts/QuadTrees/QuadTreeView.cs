using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace QuadTrees
{
    public class QuadTreeView : MonoBehaviour
    {
        private QuadTree _quadTree;

        private void Start()
        {
            _quadTree = new QuadTree(true, new Bounds(Vector3.zero, new Vector3(10, 10,10)));
            for (var i = 0; i < 50; i++)
            {
                _quadTree.Insert(new TreePoint(i, new Vector2(2, 3)+Random.insideUnitCircle));
            }
        }

        private void Update()
        {
            DrawBranches(_quadTree);
        }

        private static void DrawBranches(QuadTree quadTree)
        {
            if (quadTree.IsLeaf)
            {
                DrawBound(quadTree.Bounds);
                return;
            }

            foreach (var branch in quadTree.Branches)
            {
                DrawBranches(branch);
            }
        }
        
        private static void DrawBound(Bounds bound)
        {
            var sizeX = new Vector3(bound.size.x, 0, 0);
            var sizeY = new Vector3(0, bound.size.y, 0);

            Debug.DrawLine(bound.center - sizeX - sizeY, bound.center + sizeX - sizeY);
            Debug.DrawLine(bound.center + sizeX - sizeY, bound.center + sizeX + sizeY);
            Debug.DrawLine(bound.center + sizeX + sizeY, bound.center - sizeX + sizeY);
            Debug.DrawLine(bound.center - sizeX + sizeY, bound.center - sizeX - sizeY);
        }
    }
}