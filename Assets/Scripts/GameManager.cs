using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject draggingObject;
    public GameObject currentContainer;
    public static GameManager Instance;
    public GameObject gameOverPanel;

    public enum ToolMode { None, Pickaxe }
    public ToolMode CurrentTool = ToolMode.None;

    [Header("Economy")]
    public int coins = 0;
    public TMP_Text coinText;

    [Header("Timer")]
    public TMP_Text timerText;
    public TMP_Text bestTimeText;
    private float playTime = 0f;
    private bool isGameEnded = false;

    [Header("UI Buttons")]
    public GameObject playAgainButton;

    [Header("Wave Info")]
    public TMP_Text waveText;

    public Texture2D pickaxeCursor;

    [Header("Win Condition")]
    [Tooltip("เวลาที่ต้องรอดให้ถึง (วินาที). 15 นาที = 900 วินาที")]
    public float winTimeSeconds = 15f * 60f; // = 900

    [Header("Result UI")]
    [Tooltip("ข้อความผลลัพธ์บนหน้าจอ (เช่น Game Over / Your Win!!)")]
    public TMP_Text resultText;

    private void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (playAgainButton != null) playAgainButton.SetActive(false);
        UpdateCoinUI();

        if (bestTimeText != null) bestTimeText.text = "";
        if (resultText != null) resultText.text = ""; // เคลียร์ข้อความผลลัพธ์
    }

    public void AddCoins(int amount)
    {
        coins += Mathf.Max(0, amount);
        UpdateCoinUI();
    }

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

    public void UpdateWaveUI(int wave)
    {
        if (waveText != null)
            waveText.text = "Wave: " + wave;
    }

    public void PlaceObject()
    {
        if (draggingObject != null && currentContainer != null)
        {
            var cont = currentContainer.GetComponent<ObjectContainer>();
            var card = draggingObject.GetComponent<ObjectDragging>().card;
            int price = card.cost;

            if (!TrySpend(price)) return;

            GameObject objectGame = Instantiate(card.object_Game, cont.transform);

            var rc = objectGame.GetComponent<RobotController>();
            var dc = objectGame.GetComponent<DefenseController>();

            if (rc != null)
            {
                rc.aliens = cont.spawnPoint.aliens;
                rc.container = cont;
                rc.buildCost = price;
            }
            else if (dc != null)
            {
                dc.container = cont;
            }

            cont.isFull = true;
            cont.Highlight(false);
        }
    }

    private void Update()
    {
        if (!isGameEnded)
        {
            playTime += Time.deltaTime;

            if (timerText != null)
                timerText.text = "Time: " + FormatTime(playTime);

            if (playTime >= winTimeSeconds)
            {
                Win();
            }
        }
    }

    public void GameOver()
    {
        if (isGameEnded) return; // กันซ้ำ
        isGameEnded = true;

        float best = PlayerPrefs.GetFloat("BestTime", 0f);
        if (playTime > best)
        {
            PlayerPrefs.SetFloat("BestTime", playTime);
            PlayerPrefs.Save();
            best = playTime;
        }

        if (bestTimeText != null)
            bestTimeText.text = "Best: " + FormatTime(best);

        if (resultText != null)
            resultText.text = "Game Over";

        if (gameOverPanel) gameOverPanel.SetActive(true);
        if (playAgainButton) playAgainButton.SetActive(true);

        Time.timeScale = 0f;
    }

    public void Win()
    {
        if (isGameEnded) return; // กันซ้ำ
        isGameEnded = true;

        float best = PlayerPrefs.GetFloat("BestTime", 0f);
        if (playTime > best)
        {
            PlayerPrefs.SetFloat("BestTime", playTime);
            PlayerPrefs.Save();
            best = playTime;
        }

        if (bestTimeText != null)
            bestTimeText.text = "Best: " + FormatTime(best);

        if (resultText != null)
            resultText.text = "Your Win!!";

        if (gameOverPanel) gameOverPanel.SetActive(true);
        if (playAgainButton) playAgainButton.SetActive(true);

        Time.timeScale = 0f;
    }

    public void PlayAgain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void SetPickaxeMode(bool enabled)
    {
        CurrentTool = enabled ? ToolMode.Pickaxe : ToolMode.None;

        if (enabled && pickaxeCursor != null)
            Cursor.SetCursor(pickaxeCursor, Vector2.zero, CursorMode.Auto);
        else
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
