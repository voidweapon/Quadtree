using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTreeCollider : MonoBehaviour
{
    private bool m_isDirty = false;
    public bool IsDirty { get { return m_isDirty; } }

    [SerializeField]
    private Transform m_transform = null;
    public Transform Transform { get { return m_transform; } }

    [SerializeField]
    private Collider2D m_collider = null;
    public Collider2D Collider { get { return m_collider; } }

    private Vector3 m_lastPosition = Vector3.zero;
    public Vector3 LatePosition { get { return m_lastPosition; } }

    public Vector3 position { get { return m_transform.position; } }

    private float m_speed = 0.1f;
    public float Speed { get { return m_speed; } }
    public int RandomMoveFrame = 0;
    public Vector2 Direction = Vector2.zero;

    private Collision.QuadTree m_treeNode = null;

    public void Init()
    {
        RandomMoveFrame = Random.Range(0, 500);
        m_speed = 3 + Random.value * 7f;
        m_lastPosition = m_transform.position;
        m_isDirty = true;
    }

    public void GameUpdate()
    {
        m_isDirty = (m_lastPosition != m_transform.position);
        RandomMoveFrame++;
    }

    public void SetPosition(Vector3 newPosition)
    {
        Transform.position = newPosition;
    }

    public void GameLateUpdate()
    {
        m_lastPosition = m_transform.position;
    }

    public ref readonly Collision.QuadTree TreeNode()
    {
        return ref m_treeNode;
    }
    public void SetTreeNode(Collision.QuadTree node)
    {
        this.m_treeNode = node;
    }
}
