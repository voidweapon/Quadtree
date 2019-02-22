using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public bool EnableMove = true;

    CollisionController collisionController;
    MoverController moverController;
    SpawnController spawnController;
    SearchController searchController;
    private void Awake()
    {
        collisionController = CollisionController.Instance;
        moverController = MoverController.Instance;
        spawnController = SpawnController.Instance;
        searchController = SearchController.Instance;
    }


    // Update is called once per frame
    private void Update()
    {
        if (moverController != null && EnableMove)
            moverController.GameUpdate();

        if (collisionController != null)
            collisionController.GameUpdate();

        if (searchController != null)
            searchController.GameUpdate();

        if (spawnController != null)
            spawnController.GameUpdate();
    }

    private void LateUpdate()
    {
        if (collisionController != null)
            collisionController.GameLateUpdate();
    }

    private void FixedUpdate()
    {
        if (collisionController != null)
            collisionController.GameFixedUpdate();
    }
}
