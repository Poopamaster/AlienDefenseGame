using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject draggingObject;
    public GameObject currentContainer;
    public static GameManager Instance;
    public GameObject gameOverPanel;

    [Header("Economy")]
    public int coins = 0;
    public TMP_Text coinText;

    [Header("Timer")]
    public TMP_Text timerText;
    public TMP_Text bestTimeText;
    private float playTime = 0f;
    private bool isGameOver = false;

    [Header("UI Buttons")]
    public GameObject playAgainButton;

    [Header("Wave Info")]
    public TMP_Text waveText;

    private void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (playAgainButton != null) playAgainButton.SetActive(false);
        UpdateCoinUI();

        if (bestTimeText != null)
            bestTimeText.text = "";
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
        if (!isGameOver)
        {
            playTime += Time.unscaledDeltaTime;
            if (timerText != null)
                timerText.text = "Time: " + FormatTime(playTime);
        }
    }

    public void GameOver()
    {
        Debug.Log("GAME OVER!");
        isGameOver = true;

        float best = PlayerPrefs.GetFloat("BestTime", 0f);
        if (playTime > best)
        {
            PlayerPrefs.SetFloat("BestTime", playTime);
            PlayerPrefs.Save();
            best = playTime;
        }

        if (bestTimeText != null)
            bestTimeText.text = "Best: " + FormatTime(best);

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
}
