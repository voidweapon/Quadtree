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

        Rect northWestRect = Rect.zero;
        Rect northEastRect = Rect.zero;
        Rect southWestRect = Rect.zero;
        Rect southEastRect = Rect.zero;

        Rect boundary;

        bool haveChild = false;

        public QuadTree(Rect boundary)
        {
            objects = new List<Transform>();
            objects.Capacity = 2 * node_capacity;

            Init(boundary);
        }
        public void Init(Rect boundary)
        {
            haveChild = false;
            this.boundary = boundary;

            Vector2 childSize = boundary.size / 2;

            northWestRect = new Rect(boundary.position.x, boundary.position.y + childSize.y,
                                    childSize.x, childSize.y);

            northEastRect = new Rect(boundary.position.x + childSize.x, boundary.position.y + childSize.y,
                                    childSize.x, childSize.y);

            southWestRect = new Rect(boundary.position.x, boundary.position.y,
                                    childSize.x, childSize.y);

            southEastRect = new Rect(boundary.position.x + childSize.x, boundary.position.y,
                                    childSize.x, childSize.y);
        }

        /// <summary>
        /// 继续细分区域
        /// </summary>
        void subdivide(Transform obj)
        {
           
            for (int i = 0; i < objects.Count; i++)
            {
                if (northWestRect.Contains(objects[i].position))
                {
                    if(northWest == null)
                    {
                        northWest = CollisionController.Instance.GetTreeNodeFromPool();
                        northWest.Init(northWestRect);
                    }
                    northWest.inserToThisNode(objects[i]);
                }
                else if (northEastRect.Contains(objects[i].position))
                {
                    if (northEast == null)
                    {
                        northEast = CollisionController.Instance.GetTreeNodeFromPool();
                        northEast.Init(northEastRect);
                    }
                    northEast.inserToThisNode(objects[i]);
                }
                else if (southWestRect.Contains(objects[i].position))
                {
                    if (southWest == null)
                    {
                        southWest = CollisionController.Instance.GetTreeNodeFromPool();
                        southWest.Init(southWestRect);
                    }
                    southWest.inserToThisNode(objects[i]);
                }
                else if (southEastRect.Contains(objects[i].position))
                {
                    if (southEast == null)
                    {
                        southEast = CollisionController.Instance.GetTreeNodeFromPool();
                        southEast.Init(southEastRect);
                    }
                    southEast.inserToThisNode(objects[i]);
                }
            }

            objects.Clear();
            haveChild = true;

            if (northWestRect.Contains(obj.position))
            {
                if (northWest == null)
                {
                    northWest = CollisionController.Instance.GetTreeNodeFromPool();
                    northWest.Init(northWestRect);
                }
                northWest.inserToThisNode(obj);
            }
            else if (northEastRect.Contains(obj.position))
            {
                if (northEast == null)
                {
                    northEast = CollisionController.Instance.GetTreeNodeFromPool();
                    northEast.Init(northEastRect);
                }
                northEast.inserToThisNode(obj);
            }
            else if(southWestRect.Contains(obj.position))
            {
                if (southWest == null)
                {
                    southWest = CollisionController.Instance.GetTreeNodeFromPool();
                    southWest.Init(southWestRect);
                }
                southWest.inserToThisNode(obj);
            }
            else if(southEastRect.Contains(obj.position))
            {
                if (southEast == null)
                {
                    southEast = CollisionController.Instance.GetTreeNodeFromPool();
                    southEast.Init(southEastRect);
                }
                southEast.inserToThisNode(obj);
            }
        }

        void collapse()
        {
            if(northWest != null)
            {
                objects.AddRange(northWest.objects);
                CollisionController.Instance.RecycleTreeNode(northWest);
            }
            if (northEast != null)
            {
                objects.AddRange(northEast.objects);
                CollisionController.Instance.RecycleTreeNode(northEast);
            }
            if (southWest != null)
            {
                objects.AddRange(southWest.objects);
                CollisionController.Instance.RecycleTreeNode(southWest);
            }
            if (southEast != null)
            {
                objects.AddRange(southEast.objects);
                CollisionController.Instance.RecycleTreeNode(southEast);
            }

            northWest = null;
            northEast = null;
            southWest = null;
            southEast = null;

            haveChild = false;
        }

        public void queryRange(Rect rect, List<Transform> inRangeObject)
        {
            if (!boundary.Overlaps(rect, false))
            {
                return;
            }

            if (haveChild)
            {
                if (northWest != null)
                    northWest.queryRange(rect, inRangeObject);

                if (northEast != null)
                    northEast.queryRange(rect, inRangeObject);

                if (southWest != null)
                    southWest.queryRange(rect, inRangeObject);

                if (southEast != null)
                    southEast.queryRange(rect, inRangeObject);
            }
            else
            {
                for (int i = 0; i < objects.Count; i++)
                {
                    if (rect.Contains(objects[i].position))
                    {
                        inRangeObject.Add(objects[i]);
                    }
                }
            }
        }

        public bool insert(Transform obj)
        {
            if (!boundary.Contains(obj.position))
             {
                return false;
            }

            return inserToThisNode(obj);
        }

        private bool inserToThisNode(Transform obj)
        {
            Vector3 position = obj.position;
            if (haveChild)
            {
                if(northWest == null)
                {
                    if (northWestRect.Contains(position))
                    {
                        northWest = CollisionController.Instance.GetTreeNodeFromPool();
                        northWest.Init(northWestRect);
                        northWest.inserToThisNode(obj);
                    }
                }
                else if(northWest.insert(obj))
                {
                    return true;
                }

                if (northEast == null)
                {
                    if (northEastRect.Contains(position))
                    {
                        northEast = CollisionController.Instance.GetTreeNodeFromPool();
                        northEast.Init(northEastRect);
                        northEast.inserToThisNode(obj);
                    }
                }
                else if (northEast.insert(obj))
                {
                    return true;
                }

                if (southWest == null)
                {
                    if (southWestRect.Contains(position))
                    {
                        southWest = CollisionController.Instance.GetTreeNodeFromPool();
                        southWest.Init(southWestRect);
                        southWest.inserToThisNode(obj);
                    }
                }
                else if (southWest.insert(obj))
                {
                    return true;
                }

                if (southEast == null)
                {
                    if (southEastRect.Contains(position))
                    {
                        southEast = CollisionController.Instance.GetTreeNodeFromPool();
                        southEast.Init(southEastRect);
                        southEast.inserToThisNode(obj);
                    }
                }
                else if (southEast.insert(obj))
                {
                    return true;
                }
            }
            else
            {
                if (objects.Count < node_capacity)
                {
                    objects.Add(obj);
                    return true;
                }

               subdivide(obj);
               return true;
            }

            return false;
        }

        public bool remove(QuadTreeCollider obj)
        {
            if (!boundary.Contains(obj.LatePosition))
            {
                return false;
            }

            if (haveChild)
            {
                bool isChanged = false;
                int total = 0;
                if (northWest != null && northWest.remove(obj))
                {
                    isChanged = true;
                    total += northWest.Count();
                }
                else if (northEast != null && northEast.remove(obj))
                {
                    isChanged = true;
                    total += northEast.Count();
                }
                else if (southWest != null && southWest.remove(obj))
                {
                    isChanged = true;
                    total += southWest.Count();
                }
                else if (southEast != null && southEast.remove(obj))
                {
                    isChanged = true;
                    total += southEast.Count();
                }

                if (isChanged)
                {
                    if (total <= node_capacity)
                    {
                        collapse();
                    }
                    return true;
                }
            }
            else
            {
                if (objects.Remove(obj.Transform))
                {
                    return true;
                }
            }

            return false;
        }

        public int Count()
        {
            int total = 0;
            if (haveChild)
            {
                if(northWest != null)
                    total += northWest.Count();
                if (northEast != null)
                    total += northEast.Count();
                if (southWest != null)
                    total += southWest.Count();
                if (southEast != null)
                    total += southEast.Count();
            }
            else
            {
                total = objects.Count;
            }
            
            return total;
        }

        public void DrawBoundary()
        {

            if (haveChild)
            {
                if (northWest != null)
                    northWest.DrawBoundary();
                if (northEast != null)
                    northEast.DrawBoundary();
                if (southWest != null)
                    southWest.DrawBoundary();
                if (southEast != null)
                    southEast.DrawBoundary();
            }
            else
            {
                Gizmos.DrawWireCube(boundary.position + boundary.size / 2, boundary.size);
            }
        }

        public void Clear()
        {
            objects.Clear();
            haveChild = false;
            northWest = null;
            northEast = null;
            southWest = null;
            southEast = null;
        }
    } 
}
   