using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class SpeedToggleButton : MonoBehaviour
{
    [Header("UI")]
    public Button speedButton;
    public TMP_Text buttonLabel;

    [Header("Speed Settings")]
    public float normalSpeed = 1f;
    public float fastSpeed = 2f;

    [Header("Optional")]
    public KeyCode hotkey = KeyCode.F;

    private bool fastMode = false;

    void Awake()
    {
        if (speedButton == null) speedButton = GetComponent<Button>();
        if (buttonLabel == null) buttonLabel = GetComponentInChildren<TMP_Text>(true);
    }

    void Start()
    {
        speedButton.onClick.AddListener(ToggleSpeed);
        Time.timeScale = normalSpeed;  // เริ่ม x1
        UpdateLabel();
    }

    void Update()
    {
        if (hotkey != KeyCode.None && Input.GetKeyDown(hotkey))
            ToggleSpeed();
    }

    public void ToggleSpeed()
    {
        fastMode = !fastMode;
        Time.timeScale = fastMode ? fastSpeed : normalSpeed;
        UpdateLabel();
    }

    private void UpdateLabel()
    {
        if (buttonLabel != null)
            buttonLabel.text = fastMode ? $"Speed x{fastSpeed:0.#}" : $"Speed x{normalSpeed:0.#}";
    }
}
