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

    public GameObject prefab1;
    public GameObject prefab2;
    public Transform pool1;
    public Transform pool2;
    public BoxCollider2D bundary;

    private Vector2 m_btnSize = new Vector2(100, 50);
    private List<GameObject> m_pool_1 = new List<GameObject>();
    private List<GameObject> m_pool_2 = new List<GameObject>();
    private List<GameObject> activeObj1 = new List<GameObject>();
    private List<GameObject> activeObj2 = new List<GameObject>();

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
       
    }

    private void OnGUI()
    {
        Vector2 ScreenSize = new Vector2(Screen.width, Screen.height);
        Vector2 BtnPos = new Vector2(Screen.width - m_btnSize.x, 0);
        Vector2 offset = new Vector2(0, 5);

        if(GUI.Button(new Rect(BtnPos, m_btnSize), "Add 1000 box"))
        {
            AddCollider(ColliderType.Box, 1000);
        }
        BtnPos += (new Vector2(0, m_btnSize.y) + offset);
        if (GUI.Button(new Rect(BtnPos, m_btnSize), "Add 1000 circle"))
        {
            AddCollider(ColliderType.Circle, 1000);

        }
        BtnPos += (new Vector2(0, m_btnSize.y) + offset);
        if (GUI.Button(new Rect(BtnPos, m_btnSize), "Add 10000 box"))
        {
            AddCollider(ColliderType.Box, 10000);
        }
        BtnPos += (new Vector2(0, m_btnSize.y) + offset);
        if (GUI.Button(new Rect(BtnPos, m_btnSize), "Add 10000 circle"))
        {
            AddCollider(ColliderType.Circle, 10000);
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
        GameObject obj = null;
        Vector3 bundaryPosistion = bundary.gameObject.transform.position;
        Vector2 bundaryPosistion2D = new Vector2(bundaryPosistion.x, bundaryPosistion.y) - bundary.size / 2;

        switch (colliderType)
        {
            case ColliderType.Box:
                for (int i = 0; i < count; i++)
                {
                    obj = GetObj1();
                    obj.transform.SetParent(null);
                    obj.transform.position = RandomPosition(bundary.size) + bundaryPosistion2D;
                    CollisionController.Instance.RegisteCollider(obj);
                    activeObj1.Add(obj);
                }
                break;
            case ColliderType.Circle:
                for (int i = 0; i < count; i++)
                {
                    obj = GetObj2();
                    obj.transform.SetParent(null);
                    obj.transform.position = RandomPosition(bundary.size) + bundaryPosistion2D;
                    CollisionController.Instance.RegisteCollider(obj);
                    activeObj2.Add(obj);
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

    #region ColliderPool
    GameObject GetObj1()
    {
        GameObject obj = null;
        if (m_pool_1.Count == 0)
        {
            obj = Instantiate(prefab1, pool1);
            m_pool_1.Add(obj);
        }
        obj = m_pool_1[0];
        m_pool_1.RemoveAt(0);
        return obj;
    }
    void RecycleObj1(GameObject obj)
    {
        obj.transform.SetParent(pool1);
        m_pool_1.Add(obj);
    }

    GameObject GetObj2()
    {
        GameObject obj = null;
        if (m_pool_2.Count == 0)
        {
            obj = Instantiate(prefab2, pool2);
            m_pool_2.Add(obj);
        }
        obj = m_pool_2[0];
        m_pool_2.RemoveAt(0);
        return obj;
    }
    void RecycleObj2(GameObject obj)
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
