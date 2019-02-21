using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.ObjectModel;

public class MoverController : MonoBehaviour
{
    public BoxCollider2D boundary;
    private Rect boundaryRect;

    private static MoverController m_instance = null;
    public static MoverController Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = GameObject.FindGameObjectWithTag("MoverController").GetComponent<MoverController>();
            }
            return m_instance;
        }
    }

    private void Awake()
    {
        Vector2 rectPos = new Vector2(boundary.transform.position.x, boundary.transform.position.y);
        boundaryRect = new Rect(rectPos - (boundary.size / 2), boundary.size);
    }

    public void GameUpdate()
    {
        var colliders = CollisionController.Instance.GetQuadTreeColliders();
        Vector2 curentPosition = Vector2.zero;
        Vector2 direct = Vector2.zero;
        Vector3 nowPos = Vector3.zero;
        float moveLength = 0;
        foreach (var item in colliders)
        {
            if(item.RandomMoveFrame > 500 || item.Direction == Vector2.zero)
            {
                item.Direction = Random.insideUnitCircle;
                item.RandomMoveFrame = 0;   
            }
            moveLength = item.Speed * Time.deltaTime;
            direct = item.Direction;
            curentPosition = new Vector2(item.Transform.position.x, item.Transform.position.y);
            nowPos = curentPosition + item.Direction * moveLength;

            while (!boundaryRect.Contains(nowPos))
            {
                direct = Random.insideUnitCircle;
                nowPos = curentPosition + direct * moveLength;
            }

            item.Direction = direct;
            item.SetPosition(nowPos);

        }
    }

}
