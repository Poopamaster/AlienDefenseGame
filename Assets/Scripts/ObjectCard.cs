using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ObjectCard : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Prefab / Drag")]
    public GameObject object_Drag;
    public GameObject object_Game;
    public Canvas canvas;
    [HideInInspector] public GameObject objectDragInstance;

    private GameManager gameManager;

    [Header("Economy")]
    public int cost = 100;

    [Header("Cooldown")]
    [Tooltip("เวลาคูลดาวน์ของการ์ดใบนี้ (วินาที)")]
    public float cooldownTime = 5f;

    // ใช้ค่า -INF หมายถึง "พร้อมใช้งาน" (ยังไม่เริ่มคูลดาวน์)
    private float nextAvailableTime = float.NegativeInfinity;

    [Header("UI")]
    [Tooltip("Image ทับการ์ด (ตั้ง Image Type = Filled)")]
    public Image cooldownOverlay;       // ต้องเป็นลูกของการ์ดนี้
    public TMP_Text cooldownText;       // ตัวเลขบนการ์ด

    void Awake()
    {
        gameManager = GameManager.Instance;
        SetReadyVisuals();              // บังคับปิด overlay ตั้งแต่โหลด
    }

    void OnEnable()
    {
        SetReadyVisuals();              // กันเคส prefab/enable ใหม่แล้วติดเทา
        nextAvailableTime = float.NegativeInfinity;
    }

    void Update()
    {
        // ยังไม่เคยเริ่มคูลดาวน์ -> ซ่อน UI
        if (float.IsNegativeInfinity(nextAvailableTime))
        {
            SetReadyVisuals();
            return;
        }

        float remain = nextAvailableTime - Time.time;

        if (remain > 0f)
        {
            // แสดงการนับถอยหลัง
            if (cooldownOverlay)
            {
                cooldownOverlay.enabled = true;
                cooldownOverlay.raycastTarget = true; // บล็อกลาก/คลิกตอนคูลดาวน์
                cooldownOverlay.fillAmount = Mathf.Clamp01(remain / cooldownTime);
            }
            if (cooldownText)
            {
                cooldownText.enabled = true;
                cooldownText.text = Mathf.CeilToInt(remain).ToString();
            }
        }
        else
        {
            // พร้อมใช้งานอีกครั้ง
            SetReadyVisuals();
            nextAvailableTime = float.NegativeInfinity;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (gameManager == null)
            gameManager = GameManager.Instance ?? FindObjectOfType<GameManager>();

        if (gameManager == null)
        {
            Debug.LogError("[ObjectCard] GameManager not found.");
            return;
        }

        if (!IsReady()) return;
        if (gameManager.coins < cost) return;

        if (object_Drag == null)
        {
            Debug.LogError("[ObjectCard] object_Drag is not assigned.");
            return;
        }

        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("[ObjectCard] Canvas is not assigned and not found in parents.");
                return;
            }
        }

        objectDragInstance = Instantiate(object_Drag, canvas.transform);
        objectDragInstance.transform.position = eventData.position; // ใช้ตำแหน่งจาก event

        var drag = objectDragInstance.GetComponent<ObjectDragging>();
        if (drag == null)
        {
            Debug.LogError("[ObjectCard] ObjectDragging component missing on object_Drag prefab.");
            Destroy(objectDragInstance);
            objectDragInstance = null;
            return;
        }

        drag.card = this;
        gameManager.draggingObject = objectDragInstance;
    }


    public void OnDrag(PointerEventData eventData)
    {
        if (objectDragInstance != null)
            objectDragInstance.transform.position = Input.mousePosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (objectDragInstance == null) return;

        gameManager.PlaceObject();

        gameManager.draggingObject = null;
        Destroy(objectDragInstance);
    }

    public void StartCooldownNow()
    {
        nextAvailableTime = Time.time + cooldownTime;

        if (cooldownOverlay)
        {
            cooldownOverlay.enabled = true;
            cooldownOverlay.raycastTarget = true;
            cooldownOverlay.fillAmount = 1f; // เริ่มจากเต็มแล้วค่อย ๆ ลด
        }
        if (cooldownText)
        {
            cooldownText.enabled = true;
            cooldownText.text = Mathf.CeilToInt(cooldownTime).ToString();
        }
    }

    public bool IsReady()
    {
        return float.IsNegativeInfinity(nextAvailableTime) || Time.time >= nextAvailableTime;
    }

    private void SetReadyVisuals()
    {
        if (cooldownOverlay)
        {
            cooldownOverlay.enabled = false;
            cooldownOverlay.raycastTarget = false;
            cooldownOverlay.fillAmount = 0f;
        }
        if (cooldownText)
        {
            cooldownText.enabled = false;
            cooldownText.text = "";
        }
    }
}
