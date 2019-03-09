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

        int objectCount = 0;
        bool isSubdivided = false;

        public QuadTree(Rect boundary)
        {
            objects = new List<Transform>();
            //+1 是保证细分时添加进待加对象时，不出发容量调整
            objects.Capacity = 2 * (node_capacity + 1);

            Init(boundary);
        }
        public void Init(Rect boundary)
        {
            isSubdivided = false;
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
            objects.Add(obj);
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
            isSubdivided = true;
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
            isSubdivided = false;
        }

        public void queryRange(Rect rect, List<Transform> inRangeObject)
        {
            if (!boundary.Overlaps(rect, false))
            {
                return;
            }

            if (isSubdivided)
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
            if (isSubdivided)
            {
                if (northWestRect.Contains(position))
                {
                    if (northWest == null)
                    {
                        northWest = CollisionController.Instance.GetTreeNodeFromPool();
                        northWest.Init(northWestRect);
                    }
                    northWest.inserToThisNode(obj);
                }
                else if (northEastRect.Contains(position))
                {
                    if (northEast == null)
                    {
                        northEast = CollisionController.Instance.GetTreeNodeFromPool();
                        northEast.Init(northEastRect);
                    }
                    northEast.inserToThisNode(obj);
                }
                else if (southWestRect.Contains(position))
                {
                    if (southWest == null)
                    {
                        southWest = CollisionController.Instance.GetTreeNodeFromPool();
                        southWest.Init(southWestRect);
                    }
                    southWest.inserToThisNode(obj);
                }
                else if (southEastRect.Contains(position))
                {
                    if (southEast == null)
                    {
                        southEast = CollisionController.Instance.GetTreeNodeFromPool();
                        southEast.Init(southEastRect);
                    }
                    southEast.inserToThisNode(obj);
                }
            }
            else
            {
                if (objects.Count < node_capacity)
                {
                    objects.Add(obj);
                }
                else
                {
                    subdivide(obj);
                }
            }
            objectCount++;
            return true;
        }

        public bool remove(QuadTreeCollider obj)
        {
            if (!boundary.Contains(obj.LatePosition))
            {
                return false;
            }

            bool haveChange = false;
            if(northWest != null && northWest.remove(obj))
            {
                haveChange = true;
                if(northWest.Count() == 0)
                {
                    CollisionController.Instance.RecycleTreeNode(northWest);
                    northWest = null;
                }
            }
            else if (northEast != null && northEast.remove(obj))
            {
                haveChange = true;
                if (northEast.Count() == 0)
                {
                    CollisionController.Instance.RecycleTreeNode(northEast);
                    northEast = null;
                }
            }
            else if (southWest != null && southWest.remove(obj))
            {
                haveChange = true;
                if (southWest.Count() == 0)
                {
                    CollisionController.Instance.RecycleTreeNode(southWest);
                    southWest = null;
                }
            }
            else if (southEast != null && southEast.remove(obj))
            {
                haveChange = true;
                if (southEast.Count() == 0)
                {
                    CollisionController.Instance.RecycleTreeNode(southEast);
                    southEast = null;
                }
            }
            if (!haveChange)
            {
                objects.Remove(obj.Transform);
            }

            objectCount--;
            if(objectCount < node_capacity)
            {
                collapse();
            }

            return true;
        }

        public int Count()
        {          
            return objectCount;
        }

        public void DrawBoundary()
        {
            bool haveChild = false;
            if (northWest != null)
            {
                northWest.DrawBoundary();
                haveChild = true;
            }
            if (northEast != null)
            {
                northEast.DrawBoundary();
                haveChild = true;
            }
            if (southWest != null)
            {
                southWest.DrawBoundary();
                haveChild = true;
            }
            if (southEast != null)
            {
                southEast.DrawBoundary();
                haveChild = true;
            }

            if (!haveChild)
            {
                Gizmos.DrawWireCube(boundary.position + boundary.size / 2, boundary.size);
            }
        }

        public void Clear()
        {
            objects.Clear();
            objectCount = 0;
            northWest = null;
            northEast = null;
            southWest = null;
            southEast = null;
        }
    } 
}
   