using System.Collections;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    public enum ColliderType
    {
        Box,
        Circle,
    }
    public struct AddOrder
    {
        public ColliderType colliderType;
        public int count;
    }

    public QuadTreeCollider prefab1;
    public QuadTreeCollider prefab2;
    public Transform pool1;
    public Transform pool2;
    public BoxCollider2D bundary;

    private Vector2 m_btnSize = new Vector2(100, 50);

    private List<QuadTreeCollider> m_pool_1 = new List<QuadTreeCollider>();
    private List<QuadTreeCollider> m_pool_2 = new List<QuadTreeCollider>();
    private List<QuadTreeCollider> activeObj1 = new List<QuadTreeCollider>();
    private List<QuadTreeCollider> activeObj2 = new List<QuadTreeCollider>();

    private Queue<AddOrder> addOrders = new Queue<AddOrder>();

    private static SpawnController m_instance = null;
    public static SpawnController Instance {
        get {
            if(m_instance == null)
            {
                m_instance = GameObject.FindGameObjectWithTag("SpawnController").GetComponent<SpawnController>();
            }
            return m_instance;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        InitObjectPool();
    }

    private void InitObjectPool()
    {
        m_pool_1.Capacity = 4000;
        m_pool_2.Capacity = 4000;
        activeObj1.Capacity = 1500;
        activeObj2.Capacity = 1500;

        QuadTreeCollider obj = null;
        for (int i = 0; i < 3000; i++)
        {
            obj = Instantiate(prefab1, pool1);
            m_pool_1.Add(obj);
        }

        for (int i = 0; i < 3000; i++)
        {
            obj = Instantiate(prefab2, pool2);
            m_pool_2.Add(obj);
        }
    }

    private void OnGUI()
    {
        Vector2 ScreenSize = new Vector2(Screen.width, Screen.height);
        Vector2 BtnPos = new Vector2(Screen.width - m_btnSize.x, 0);
        Vector2 offset = new Vector2(0, 5);

        if(GUI.Button(new Rect(BtnPos, m_btnSize), "Add 5 box"))
        {
            AddCollider(ColliderType.Box, 5);
        }
        BtnPos += (new Vector2(0, m_btnSize.y) + offset);
        if (GUI.Button(new Rect(BtnPos, m_btnSize), "Add 5 circle"))
        {
            AddCollider(ColliderType.Circle, 5);

        }
        BtnPos += (new Vector2(0, m_btnSize.y) + offset);
        if (GUI.Button(new Rect(BtnPos, m_btnSize), "Add 1000 box"))
        {
            AddCollider(ColliderType.Box, 1000);
        }
        BtnPos += (new Vector2(0, m_btnSize.y) + offset);
        if (GUI.Button(new Rect(BtnPos, m_btnSize), "Add 1000 circle"))
        {
            AddCollider(ColliderType.Circle, 1000);
        }
        BtnPos += (new Vector2(0, m_btnSize.y) + offset);
        if (GUI.Button(new Rect(BtnPos, m_btnSize), "Add 1 box"))
        {
            AddCollider(ColliderType.Box, 1);
        }
        BtnPos += (new Vector2(0, m_btnSize.y) + offset);
        if (GUI.Button(new Rect(BtnPos, m_btnSize), "Clear"))
        {
            ClearCollider();
        }        
    }

    private void OnDrawGizmos()
    {
        Color oldColor = Gizmos.color;

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Vector3.zero, bundary.size);

        Gizmos.color = Color.black;
        if(CollisionController.Instance.QuadTreeRoot != null)
            CollisionController.Instance.QuadTreeRoot.DrawBoundary();

        Gizmos.color = oldColor;
    }

    void AddCollider(ColliderType colliderType, int count)
    {
        addOrders.Enqueue(new AddOrder
        {
            colliderType = colliderType,
            count = count,
        });
    }

    private void HandleAddOrder(ColliderType colliderType, int count)
    {
        QuadTreeCollider objTfm = null;
        Vector3 bundaryPosistion = bundary.gameObject.transform.position;
        Vector2 bundaryPosistion2D = new Vector2(bundaryPosistion.x, bundaryPosistion.y) - bundary.size / 2;
        List<QuadTreeCollider> newObj = new List<QuadTreeCollider>();
        newObj.Capacity = count;
        switch (colliderType)
        {
            case ColliderType.Box:
                for (int i = 0; i < count; i++)
                {
                    objTfm = GetObj1();
                    objTfm.Transform.SetParent(null);
                    objTfm.Transform.position = RandomPosition(bundary.size) + bundaryPosistion2D;
                    objTfm.Init();
                    CollisionController.Instance.RegisteCollider(objTfm);
                    activeObj1.Add(objTfm);
                    newObj.Add(objTfm);
                }
                break;
            case ColliderType.Circle:
                for (int i = 0; i < count; i++)
                {
                    objTfm = GetObj2();
                    objTfm.Transform.SetParent(null);
                    objTfm.Transform.position = RandomPosition(bundary.size) + bundaryPosistion2D;
                    objTfm.Init();
                    CollisionController.Instance.RegisteCollider(objTfm);
                    activeObj2.Add(objTfm);
                    newObj.Add(objTfm);
                }
                break;
            default:
                break;
        }
    }

    void ClearCollider()
    {
        for (int i = 0; i < activeObj1.Count; i++)
        {
            RecycleObj1(activeObj1[i]);
            CollisionController.Instance.UnRegisteCollider(activeObj1[i]);
        }
        for (int i = 0; i < activeObj2.Count; i++)
        {
            RecycleObj2(activeObj2[i]);
            CollisionController.Instance.UnRegisteCollider(activeObj2[i]);
        }
    }

    public void GameUpdate()
    {
        AddOrder order;
        while (addOrders.Count > 0)
        {
            order = addOrders.Dequeue();
            HandleAddOrder(order.colliderType, order.count);
        }
    }

    #region ColliderPool
    QuadTreeCollider GetObj1()
    {
        QuadTreeCollider tsfm = null;
        if (m_pool_1.Count == 0)
        {
            tsfm = Instantiate(prefab1, pool1);
            m_pool_1.Add(tsfm);
        }
        tsfm = m_pool_1[0];
        m_pool_1.RemoveAt(0);
        return tsfm;
    }
    void RecycleObj1(QuadTreeCollider obj)
    {
        obj.transform.SetParent(pool1);
        m_pool_1.Add(obj);
    }

    QuadTreeCollider GetObj2()
    {
        QuadTreeCollider tsfm = null;
        if (m_pool_2.Count == 0)
        {
            tsfm = Instantiate(prefab2, pool2);
            m_pool_2.Add(tsfm);
        }
        tsfm = m_pool_2[0];
        m_pool_2.RemoveAt(0);
        return tsfm;
    }
    void RecycleObj2(QuadTreeCollider obj)
    {
        obj.transform.SetParent(pool2);
        m_pool_2.Add(obj);
    }
    #endregion

    Vector2 RandomPosition(Vector2 bundary)
    {
        return new Vector2(Random.value * bundary.x, Random.value * bundary.y);
    }
}
