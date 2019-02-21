using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public bool EnableMove = true;

    CollisionController collisionController;
    MoverController moverController;
    SpawnController SpawnController;
    private void Awake()
    {
        collisionController = CollisionController.Instance;
        moverController = MoverController.Instance;
        SpawnController = SpawnController.Instance;
    }


    // Update is called once per frame
    private void Update()
    {
        if (moverController != null && EnableMove)
            moverController.GameUpdate();

        if (collisionController != null)
            collisionController.GameUpdate();

        if (SpawnController != null)
            SpawnController.GameUpdate();
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
