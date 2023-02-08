using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QuadTrees
{
    public class QuadTree
    {
        private readonly List<TreePoint> _branchObjects = new List<TreePoint>();
        private readonly QuadTree _parent;
        private bool _isRoot;
        private bool _isLeaf = true;
        private int _depth = 0;

        private const int MaxCapacity = 2;
        private const int MaxDepth = 10;
        
        public bool IsLeaf => _isLeaf;
        public Bounds Bounds { get; private set; }
        public QuadTree[] Branches { get; private set; } = new QuadTree[4];
        public List<TreePoint> QuadObjects { get; } = new List<TreePoint>();

        public QuadTree(bool isRoot, Bounds bounds)
        {
            _isRoot = isRoot;
            Bounds = bounds;
        }

        private QuadTree(bool isRoot, Bounds bounds, QuadTree parent,int depth)
        {
            Bounds = bounds;
            _isRoot = isRoot;
            _parent = parent;
            _depth = depth;
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
            
            point.leaf = this;
            QuadObjects.Add(point);

            if (QuadObjects.Count > MaxCapacity && _depth<=MaxDepth)
            {
                Subdivide();
            }

        }

        public void Remove(TreePoint point)
        {
            if (IsLeaf)
            {
                if (QuadObjects.Count(p => p.id == point.id) <= 0) return;
                QuadObjects.RemoveAll(p=>p.id==point.id);
                _parent?.UpdateBranches();
                return;
            }

            foreach (var branch in Branches)
            {
                branch.Remove(point);
            }
            
        }

        public void UpdateObject(TreePoint point)
        {
            if (point.leaf.Bounds.Contains(point.position)) return;
            point.leaf.Remove(point);
            Insert(point);
        }

        private void UpdateBranches()
        {
            if(IsLeaf) return;
            _branchObjects.Clear();
            for (var i = 0; i < 4; i++)
            {
                foreach (var obj in Branches[i].QuadObjects)
                {
                    _branchObjects.Add(obj);
                }
            }

            if (_branchObjects.Count >= MaxCapacity) return;
            Connect();
        }

        private void Connect()
        {
            //for (var i = 0; i < 4; i++) Branches[i] = null;
            _isLeaf = true;
            QuadObjects.Clear();
            for (var index = 0; index < _branchObjects.Count; index++)
            {
                var obj = _branchObjects[index];
                obj.leaf = this;
                QuadObjects.Add(obj);
            }
            _parent?.UpdateBranches();
        }

        private void Subdivide()
        {
            var x = Bounds.center.x;
            var y = Bounds.center.y;
            var size = Bounds.extents;
            size.z = 10;
            
            if (Branches[1] is null)
            {
                for (var i = 0; i < 4; i++)
                {
                    var newBounds =
                        new Bounds(new Vector3(
                                x - size.x / 2 + size.x * (i % 2),
                                y - size.y / 2 + size.y * (i / 2),
                                0),
                            size);
                    Branches[i] = new QuadTree(false, newBounds, this, _depth + 1);
                }
            }

            foreach (var obj in QuadObjects)
            {
                InsertAllBranches(obj);
            }

            QuadObjects.Clear();
            _isLeaf = false;
        }

        private void InsertAllBranches(TreePoint point)
        {
            for (var i = 0; i < 4; i++)
            {
                Branches[i].Insert(point);
            }
        }
    }

    public class TreePoint
    {
        public int id;
        public Vector3 position;
        public QuadTree leaf;

        public TreePoint(int id,Vector3 position)
        {
            this.id = id;
            this.position = position;
            leaf = null;
        }
    }
}