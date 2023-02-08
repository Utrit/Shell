using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace QuadTrees
{
    public class QuadTreeView : MonoBehaviour
    {
        private QuadTree _quadTree;
        private List<TreePoint> _points;
        private float shift;
        
        private void Start()
        {
            _quadTree = new QuadTree(true, new Bounds(Vector3.zero, new Vector3(100, 100,10)));
            // for (var i = 0; i < 100000; i++)
            // {
            //     _quadTree.Insert(new TreePoint(i, Vector2.zero+Random.insideUnitCircle*5));
            // }
            _points = new List<TreePoint>();
            for (var i = 0; i < 300; i++)
            {
                var buffer = new TreePoint(i, Random.insideUnitCircle);
                _quadTree.Insert(buffer);
                _points.Add(buffer);
            }
        }

        private void Update()
        {
            shift += 0.01f;
            for (int i = 0; i < _points.Count; i++)
            {
                _points[i].position = new Vector3((shift) % 6.28f - 3.14f, Mathf.Sin(shift), 0);
                _quadTree.UpdateObject(_points[i]);
            }
            
            //DrawBranches(_quadTree);
        }

        private static void DrawBranches(QuadTree quadTree)
        {
            
            if (quadTree.IsLeaf)
            {
                DrawBound(quadTree.Bounds);
                foreach (var fo in quadTree.QuadObjects)
                {
                    Debug.DrawLine(fo.position,fo.position+Vector3.forward);
                }
                return;
            }

            foreach (var branch in quadTree.Branches)
            {
                DrawBranches(branch);
            }
        }
        
        private static void DrawBound(Bounds bound)
        {
            var sizeX = new Vector3(bound.extents.x, 0, 0);
            var sizeY = new Vector3(0, bound.extents.y, 0);

            Debug.DrawLine(bound.center - sizeX - sizeY, bound.center + sizeX - sizeY);
            Debug.DrawLine(bound.center + sizeX - sizeY, bound.center + sizeX + sizeY);
            Debug.DrawLine(bound.center + sizeX + sizeY, bound.center - sizeX + sizeY);
            Debug.DrawLine(bound.center - sizeX + sizeY, bound.center - sizeX - sizeY);
        }
    }
}