using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Collision;
using System.Collections.ObjectModel;

public class CollisionController : MonoBehaviour
{
    public QuadTree QuadTreeRoot;
    public BoxCollider2D boundary;

    private List<QuadTree> treeNodePool = new List<QuadTree>();
    private List<QuadTreeCollider> colliders = new List<QuadTreeCollider>();
    private List<QuadTreeCollider> dirtyColliders = new List<QuadTreeCollider>();


    private static CollisionController m_instance = null;
    public static CollisionController Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = GameObject.FindGameObjectWithTag("CollisionController").GetComponent<CollisionController>();
            }
            return m_instance;
        }
    }

    private void Awake()
    {
        Vector2 rectPos = new Vector2(boundary.transform.position.x, boundary.transform.position.y);
        QuadTreeRoot = new QuadTree(new Rect(rectPos - (boundary.size / 2), boundary.size));

        treeNodePool.Capacity = 1000;
        colliders.Capacity = 2000;
        dirtyColliders.Capacity = 2000;
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 1000; i++)
        {
            treeNodePool.Add(new QuadTree(Rect.zero));
        }
    }


    public void RegisteCollider(QuadTreeCollider obj)
    {
        colliders.Add(obj);
        QuadTreeRoot.insert(obj.Transform);
    }

    public void UnRegisteCollider(QuadTreeCollider obj)
    {
        QuadTreeRoot.remove(obj);
        colliders.Remove(obj);
    }

    public void GameFixedUpdate()
    {

    }

    public void GameUpdate()
    {
        dirtyColliders.Clear();

        foreach (var item in colliders)
        {
            item.GameUpdate();
        }

        foreach (var item in colliders)
        {
            if (item.IsDirty)
            {
                dirtyColliders.Add(item);
            }
        }

        foreach (var item in dirtyColliders)
        {
            QuadTreeRoot.remove(item);
        }
        foreach (var item in dirtyColliders)
        {
            QuadTreeRoot.insert(item.transform);
        }
    }

    public void GameLateUpdate()
    {
        foreach (var item in colliders)
        {
            item.GameLateUpdate();
        }
    }

    public QuadTree GetTreeNodeFromPool()
    {
        if(treeNodePool.Count == 0)
        {
            for (int i = 0; i < 10; i++)
            {
                treeNodePool.Add(new QuadTree(Rect.zero));
            }
        }

        QuadTree node = treeNodePool[0];
        treeNodePool.RemoveAt(0);

        return node;
    }

    public void RecycleTreeNode(QuadTree node)
    {
        node.Clear();
        treeNodePool.Add(node);
    }

    public ReadOnlyCollection<QuadTreeCollider> GetQuadTreeColliders()
    {
        return colliders.AsReadOnly();
    }
}
