using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject draggingObject;
    public GameObject currentContainer;
    public static GameManager Instance;
    public GameObject gameOverPanel;

    [Header("Economy")]
    public int coins = 0;          // เงินปัจจุบัน
    public TMP_Text coinText;      // ใส่ Text TMP ใน Inspector

    private void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        UpdateCoinUI();
    }

    // เพิ่มเงินตอนฆ่าเอเลี่ยน
    public void AddCoins(int amount)
    {
        coins += Mathf.Max(0, amount);
        UpdateCoinUI();
    }

    // พยายามใช้เงิน (พอ = true, ไม่พอ = false)
    public bool TrySpend(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            UpdateCoinUI();
            return true;
        }
        return false;
    }

    private void UpdateCoinUI()
    {
        if (coinText) coinText.text = coins.ToString();
    }

    public void PlaceObject()
    {
        if (draggingObject != null && currentContainer != null)
        {
            var cont = currentContainer.GetComponent<ObjectContainer>();
            var card = draggingObject.GetComponent<ObjectDragging>().card; // การ์ดที่ลากอยู่
            int price = card.cost;                                         // ราคาโรบ็อต

            // เงินไม่พอ → ไม่วาง
            if (!TrySpend(price)) return;

            GameObject objectGame = Instantiate(card.object_Game, cont.transform);

            var rc = objectGame.GetComponent<RobotController>();
            rc.aliens = cont.spawnPoint.aliens;
            rc.container = cont;

            cont.isFull = true;
            cont.Highlight(false);
        }
    }

    public void GameOver()
    {
        Debug.Log("GAME OVER!");
        if (gameOverPanel) gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }
}
