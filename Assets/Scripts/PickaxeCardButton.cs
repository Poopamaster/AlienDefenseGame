using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PickaxeCardButton : MonoBehaviour
{
    Button _btn;
    Image _img;

    public Color normalColor = Color.white;
    public Color activeColor = Color.green;

    void Awake()
    {
        _btn = GetComponent<Button>();
        _img = GetComponent<Image>();

        _btn.onClick.RemoveAllListeners();
        _btn.onClick.AddListener(OnClickPickaxe);
    }

    void Update()
    {
        if (_img == null) return;

        // อัปเดตสีปุ่มทุกเฟรมตามสถานะโหมด
        if (GameManager.Instance != null && GameManager.Instance.CurrentTool == GameManager.ToolMode.Pickaxe)
            _img.color = activeColor;
        else
            _img.color = normalColor;
    }

    void OnClickPickaxe()
    {
        if (GameManager.Instance == null) return;

        bool enable = GameManager.Instance.CurrentTool != GameManager.ToolMode.Pickaxe;
        GameManager.Instance.SetPickaxeMode(enable);
    }
}
