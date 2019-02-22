using System.Collections;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;

namespace Collision
{
    public class QuadTree
    {
        const int node_capacity = 4;
        public List<Transform> objects = null;

        QuadTree northWest = null;
        QuadTree northEast = null;
        QuadTree southWest = null;
        QuadTree southEast = null;

        Rect boundary;

        public QuadTree(Rect boundary)
        {
            //objects = new GameObject[node_capacity];
            objects = new List<Transform>();
            objects.Capacity = 2 * node_capacity;
            //objects.Capacity = node_capacity;

            this.boundary = boundary;
        }
        public void Init(Rect boundary)
        {
            this.boundary = boundary;
        }

        /// <summary>
        /// 继续细分区域
        /// </summary>
        void subdivide()
        {
            Vector2 childSize = boundary.size / 2;
            northWest = CollisionController.Instance.GetTreeNodeFromPool();
            northWest.Init(new Rect(boundary.position.x,               boundary.position.y + childSize.y, 
                                    childSize.x, childSize.y));

            northEast = CollisionController.Instance.GetTreeNodeFromPool();
            northEast.Init(new Rect(boundary.position.x + childSize.x, boundary.position.y + childSize.y,
                                    childSize.x, childSize.y));

            southWest = CollisionController.Instance.GetTreeNodeFromPool();
            southWest.Init(new Rect(boundary.position.x, boundary.position.y,
                                    childSize.x, childSize.y));

            southEast = CollisionController.Instance.GetTreeNodeFromPool();
            southEast.Init(new Rect(boundary.position.x + childSize.x, boundary.position.y,
                                    childSize.x, childSize.y));

            for (int i = 0; i < objects.Count; i++)
            {
                if (northWest.insert(objects[i])) continue;
                if (northEast.insert(objects[i])) continue;
                if (southWest.insert(objects[i])) continue;
                if (southEast.insert(objects[i])) continue;
            }
            objects.Clear();
        }

        void collapse()
        {
            objects.AddRange(northWest.objects);
            objects.AddRange(northEast.objects);
            objects.AddRange(southWest.objects);
            objects.AddRange(southEast.objects);

            CollisionController.Instance.RecycleTreeNode(northWest);
            CollisionController.Instance.RecycleTreeNode(northEast);
            CollisionController.Instance.RecycleTreeNode(southWest);
            CollisionController.Instance.RecycleTreeNode(southEast);
            northWest = null;
            northEast = null;
            southWest = null;
            southEast = null;
        }

        public void queryRange(Rect rect, List<Transform> inRangeObject)
        {
            if (!boundary.Overlaps(rect, false))
            {
                return;
            }

            for (int i = 0; i < objects.Count; i++)
            {
                if (rect.Contains(objects[i].position))
                {
                    inRangeObject.Add(objects[i]);
                }
            }

            if (northWest == null)
                return;

            northWest.queryRange(rect, inRangeObject);
            northEast.queryRange(rect, inRangeObject);
            southWest.queryRange(rect, inRangeObject);
            southEast.queryRange(rect, inRangeObject);
        }

        public bool insert(Transform obj)
        {
            if (!boundary.Contains(obj.position))
             {
                return false;
            }

            if(objects.Count < node_capacity && northWest == null)
            {
                objects.Add(obj);
                return true;
            }

            if(northWest == null)
            {
                subdivide();
            }

            if (northWest.insert(obj)) return true;
            if (northEast.insert(obj)) return true;
            if (southWest.insert(obj)) return true;
            if (southEast.insert(obj)) return true;

            return false;
        }

        public bool remove(QuadTreeCollider obj)
        {
            if (!boundary.Contains(obj.LatePosition))
            {
                return false;
            }
            if (objects.Remove(obj.Transform))
            {
                return true;
            }

            if(northWest == null)
            {
                return false;
            }

            bool isChanged = false;
            if (northWest.remove(obj))
            {
                isChanged = true;
            }
            else if (northEast.remove(obj))
            {
                isChanged = true;
            }
            else if (southWest.remove(obj))
            {
                isChanged = true;
            }
            else if (southEast.remove(obj))
            {
                isChanged = true;
            }
            if (isChanged)
            {
                int total = 0;
                total += northWest.Count();
                total += northEast.Count();
                total += southWest.Count();
                total += southEast.Count();
                if(total <= node_capacity)
                {
                    collapse();
                }
            }

            return false;
        }

        public int Count()
        {
            int total = 0;
            
            total += objects.Count;
            if (northWest == null) return total;

            total += northWest.Count();
            total += northEast.Count();
            total += southWest.Count();
            total += southEast.Count();

            return total;
        }

        public void DrawBoundary()
        {
            if (northWest == null)
            {
                Gizmos.DrawWireCube(boundary.position + boundary.size / 2, boundary.size);
                return;
            }
            else
            {
                northWest.DrawBoundary();
                northEast.DrawBoundary();
                southWest.DrawBoundary();
                southEast.DrawBoundary();
            }
        }

        public void Clear()
        {
            objects.Clear();
        }
    } 
}
