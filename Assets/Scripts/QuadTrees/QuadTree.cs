using System.Collections.Generic;
using UnityEngine;

namespace QuadTrees
{
    public class QuadTree
    {
        private readonly List<TreePoint> _allObjects = new List<TreePoint>();
        private readonly List<TreePoint> _quadObjects = new List<TreePoint>();
        private readonly List<TreePoint> _branchObjects = new List<TreePoint>();
        private readonly QuadTree _parent;
        private bool _isRoot;

        private const int MaxCapacity = 4;

        public bool IsLeaf => Branches[0] is null;
        public Bounds Bounds { get; private set; }
        public QuadTree[] Branches { get; private set; } = new QuadTree[4];

        public QuadTree(bool isRoot, Bounds bounds)
        {
            _isRoot = isRoot;
            Bounds = bounds;
        }

        public QuadTree(bool isRoot, Bounds bounds, List<TreePoint> allTreeObjects, QuadTree parent)
        {
            Bounds = bounds;
            _isRoot = isRoot;
            _parent = parent;
            _allObjects = allTreeObjects;
        }

        public void Insert(TreePoint point)
        {
            if (!Bounds.Contains(point.position))
            {
                return;
            }
            
            if (!IsLeaf)
            {
                InsertAllBranches(point);
                return;
            }
            
            if (_quadObjects.Count + 1 > MaxCapacity)
            {
                _quadObjects.Add(point);
                Subdivide();
                return;
            }

            _quadObjects.Add(point);
            _allObjects.Add(point);
        }

        public void Remove(TreePoint point)
        {
            if (!_allObjects.Contains(point))
            {
                return;
            }

            if (IsLeaf)
            {
                if (_quadObjects.Contains(point))
                {
                    _quadObjects.Remove(point);
                    _allObjects.Remove(point);
                    _parent.UpdateBranches();
                    return;
                }
            }

            foreach (var branch in Branches)
            {
                branch.Remove(point);
            }
            
        }

        public void UpdateObject(TreePoint point)
        {

        }

        private void UpdateBranches()
        {
            _branchObjects.Clear();
            for (var i = 0; i < 4; i++)
            {
                foreach (var obj in Branches[i]._quadObjects)
                {
                    _branchObjects.Add(obj);
                }
            }

            if (_branchObjects.Count >= MaxCapacity) return;
            Connect();
        }

        private void Connect()
        {
            for (int i = 0; i < 4; i++) Branches[i] = null;
            _quadObjects.Clear();
            foreach (var obj in _branchObjects)
            {
                _quadObjects.Add(obj);
            }
        }

        private void Subdivide()
        {
            var x = Bounds.center.x;
            var y = Bounds.center.y;
            var size = Bounds.extents;
            size.z = 10;
            for (var i = 0; i < 4; i++)
            {
                var newBounds =
                    new Bounds(new Vector3(
                            x - size.x + 2 * size.x * (i % 2),
                            y - size.y + 2 * size.y * (i / 2),
                            0),
                        size);
                Branches[i] = new QuadTree(false, newBounds, _allObjects, this);
            }


            for (int i = _quadObjects.Count-1; i >= 0; i--)
            {
                var quadObj = _quadObjects[i];
                _quadObjects.RemoveAt(i);
                _allObjects.Remove(quadObj);
                InsertAllBranches(quadObj);
            }
        }

        private void InsertAllBranches(TreePoint point)
        {
            for (var i = 0; i < 4; i++)
            {
                Branches[i].Insert(point);
            }
        }
    }

    public struct TreePoint
    {
        public int id;
        public Vector3 position;

        public TreePoint(int id,Vector3 position)
        {
            this.id = id;
            this.position = position;
        }
    }
}