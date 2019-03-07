using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.ObjectModel;

public class SearchController : MonoBehaviour
{
    public BoxCollider2D boundaryCollider;

    private int searchCount = 0;
    private Color SearchBoxColor = Color.red;
    private Color resultObjColor = Color.green;

    private static SearchController m_instance = null;
    private List<Rect> searchRects = new List<Rect>();
    private List<Transform> results = new List<Transform>();
    List<QuadTreeCollider> collections = null;

    public static SearchController Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = GameObject.FindGameObjectWithTag("SearchController").GetComponent<SearchController>();
            }
            return m_instance;
        }
    }

    private void Awake()
    {
        Vector2 rectPos = new Vector2(boundaryCollider.transform.position.x, boundaryCollider.transform.position.y);
        var boundaryRect = new Rect(rectPos - (boundaryCollider.size / 2), boundaryCollider.size);

        //生成搜索区域
        Vector2 pos = Vector2.zero;
        Vector2 size = Vector2.zero;
        for (int i = 0; i < searchCount; i++)
        {
            pos = RandomPosition(boundaryCollider.size) - boundaryCollider.size / 2;
            size = new Vector2((1 + Random.Range(0, 5)), (1 + Random.Range(0, 5)));
            searchRects.Add(new Rect(pos, size));
        }

        results.Capacity = 500;
    }

    public void GameUpdate()
    {
        results.Clear();
        for (int i = 0; i < searchRects.Count; i++)
        {
            CollisionController.Instance.queryRange(searchRects[i], results);
        }


        collections = CollisionController.Instance.GetQuadTreeColliders();
        foreach (var item in collections)
        {
            item.GetComponent<SpriteRenderer>().color = Color.red;
        }

        for (int i = 0; i < results.Count; i++)
        {
            results[i].GetComponent<SpriteRenderer>().color = resultObjColor;
        }
    }

    public void GameDrawGizmos()
    {
        Color oldColor = Gizmos.color;
        Gizmos.color = SearchBoxColor;
        for (int i = 0; i < searchRects.Count; i++)
        {
            Gizmos.DrawWireCube(searchRects[i].position + (searchRects[i].size / 2), searchRects[i].size);
        }
        Gizmos.color = oldColor;
    }

    Vector2 RandomPosition(Vector2 bundary)
    {
        return new Vector2(Random.value * bundary.x, Random.value * bundary.y);
    }
}
