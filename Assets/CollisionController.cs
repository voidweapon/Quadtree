using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Collision;

public class CollisionController : MonoBehaviour
{
    public QuadTree QuadTreeRoot;
    public BoxCollider2D boundary;
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
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RegisteCollider(GameObject obj)
    {
        QuadTreeRoot.insert(obj);
    }

    public void UnRegisteCollider(GameObject obj)
    {
        QuadTreeRoot.remove(obj);
    }
}
