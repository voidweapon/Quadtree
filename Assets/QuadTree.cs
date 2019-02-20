using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;

namespace Collision
{

    public class QuadTree
    {
        int node_capacity = 4;
        public List<GameObject> objects = null;

        QuadTree northWest = null;
        QuadTree northEast = null;
        QuadTree southWest = null;
        QuadTree southEast = null;

        Rect boundary;

        public QuadTree(Rect boundary)
        {
            objects = new List<GameObject>();
            this.boundary = boundary;
        }

        /// <summary>
        /// 继续细分区域
        /// </summary>
        void subdivide()
        {
            Vector2 childSize = boundary.size / 2;
            northWest = new QuadTree(new Rect(boundary.position.x,               boundary.position.y + childSize.y, 
                                    childSize.x, childSize.y));

            northEast = new QuadTree(new Rect(boundary.position.x + childSize.x, boundary.position.y + childSize.y, 
                                    childSize.x, childSize.y));

            southWest = new QuadTree(new Rect(boundary.position.x,               boundary.position.y, 
                                    childSize.x, childSize.y));

            southEast = new QuadTree(new Rect(boundary.position.x + childSize.x, boundary.position.y, 
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

            northWest = null;
            northEast = null;
            southWest = null;
            southEast = null;
        }

        public List<GameObject> queryRange(Rect rect)
        {
            List<GameObject> inRangeObject = new List<GameObject>();

            if(!boundary.Overlaps(rect, false))
            {
                return inRangeObject;
            }

            for (int i = 0; i < objects.Count; i++)
            {
                if (rect.Contains(objects[i].transform.position))
                {
                    inRangeObject.Add(objects[i]);
                }
            }

            if (northWest == null)
                return inRangeObject;

            inRangeObject.AddRange(northWest.queryRange(rect));
            inRangeObject.AddRange(northEast.queryRange(rect));
            inRangeObject.AddRange(southWest.queryRange(rect));
            inRangeObject.AddRange(southEast.queryRange(rect));

            return inRangeObject;
        }

        public bool insert(GameObject obj)
        {
            if (!boundary.Contains(obj.transform.position))
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

        public bool remove(GameObject obj)
        {
            if (!boundary.Contains(obj.transform.position))
            {
                return false;
            }
            if (objects.Remove(obj))
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
            Gizmos.DrawWireCube(boundary.position + boundary.size / 2, boundary.size);

            if (northWest == null)
            {
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
    } 
}
