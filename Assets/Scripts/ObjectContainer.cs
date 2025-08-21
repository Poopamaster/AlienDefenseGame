using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectContainer : MonoBehaviour
{
    public bool isFull;
    public GameManager gameManager;
    public Image backgroundImage;
    public SpawnPoint spawnPoint;

    private void Start()
    {
        gameManager = GameManager.Instance;
    }
    private void OnTriggerEnter2D(Collider2D c)
    {
        if (gameManager.draggingObject == null) return;
        if (!isFull && c.gameObject == gameManager.draggingObject)
        {
            gameManager.currentContainer = gameObject;
            Highlight(true);
        }
    }

    private void OnTriggerExit2D(Collider2D c)
    {
        if (gameManager.draggingObject == null) return;
        if (c.gameObject == gameManager.draggingObject)
        {
            if (gameManager.currentContainer == gameObject)
                gameManager.currentContainer = null;
            Highlight(false);
        }
    }

    public void Highlight(bool active)
{
    if (backgroundImage) backgroundImage.enabled = active;
}


}

