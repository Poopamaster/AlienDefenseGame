using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PickaxeTool : MonoBehaviour
{
    [Header("Refund")]
    [Range(0f, 1f)] public float refundPercent = 0.25f;

    [Header("SFX/VFX (optional)")]
    public AudioClip pickaxeSfx;
    [Range(0f, 1f)] public float pickaxeVolume = 1f;
    public GameObject breakFxPrefab;

    [Header("Physics (world pick)")]
    public LayerMask pickableLayers = ~0;   // ตั้งให้รวมเลเยอร์ของหุ่น/กำแพง

    Camera cam;
    EventSystem es;
    GraphicRaycaster[] raycasters;
    PointerEventData pointer;

    void Start()
    {
        cam = Camera.main;
        es = EventSystem.current;
        raycasters = FindObjectsOfType<GraphicRaycaster>(includeInactive: false);
        pointer = new PointerEventData(es);
    }

    void Update()
    {
        if (GameManager.Instance.CurrentTool != GameManager.ToolMode.Pickaxe) return;

        // คลิกขวาออกจากโหมด
        if (Input.GetMouseButtonDown(1))
        {
            GameManager.Instance.SetPickaxeMode(false);
            return;
        }

        // คลิกซ้ายขุด
        if (Input.GetMouseButtonDown(0))
        {
            // 1) ลองจาก UI ก่อน (รองรับ prefab ที่เป็น UI)
            if (TryPickaxeUI()) return;

            // 2) ไม่เจอใน UI → ยิง Physics2D ที่โลกจริง
            TryPickaxeWorld();
        }
    }

    bool TryPickaxeUI()
    {
        if (es == null || raycasters == null || raycasters.Length == 0) return false;

        pointer.position = Input.mousePosition;

        var results = new List<RaycastResult>();
        foreach (var gr in raycasters)
        {
            if (gr == null || !gr.isActiveAndEnabled) continue;

            results.Clear();
            gr.Raycast(pointer, results);
            if (results.Count == 0) continue;

            foreach (var r in results)
            {
                if (r.gameObject == null) continue;

                // หาได้ทั้งที่ตัวเอง/พาเรนต์
                var robot = r.gameObject.GetComponent<RobotController>() ??
                            r.gameObject.GetComponentInParent<RobotController>();
                if (robot != null) { RemoveRobot(robot); return true; }

                var defense = r.gameObject.GetComponent<DefenseController>() ??
                              r.gameObject.GetComponentInParent<DefenseController>();
                if (defense != null) { RemoveDefense(defense); return true; }

                // เปิดส่วนนี้ถ้าจะให้ขุด Bomb ด้วย
                // var bomb = r.gameObject.GetComponent<Bomb>() ?? r.gameObject.GetComponentInParent<Bomb>();
                // if (bomb != null) { RemoveBomb(bomb); return true; }
            }
        }
        return false;
    }

    void TryPickaxeWorld()
    {
        if (cam == null) return;

        Vector3 world = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 p = new Vector2(world.x, world.y);

        // เก็บทุกคอลไลเดอร์ ณ จุดคลิก
        var hits = Physics2D.OverlapPointAll(p, pickableLayers);
        if (hits == null || hits.Length == 0) return;

        foreach (var h in hits)
        {
            if (h == null) continue;

            // ส่วนมากคอมโพเนนต์จะอยู่ที่ root → ลอง Parent ก่อน
            var robot = h.GetComponentInParent<RobotController>() ?? h.GetComponent<RobotController>();
            if (robot != null) { RemoveRobot(robot); return; }

            var defense = h.GetComponentInParent<DefenseController>() ?? h.GetComponent<DefenseController>();
            if (defense != null) { RemoveDefense(defense); return; }

            // เปิดส่วนนี้ถ้าจะให้ขุด Bomb ด้วย
            // var bomb = h.GetComponentInParent<Bomb>() ?? h.GetComponent<Bomb>();
            // if (bomb != null) { RemoveBomb(bomb); return; }
        }
    }

    void RemoveRobot(RobotController robot)
    {
        int refund = Mathf.Max(0, Mathf.RoundToInt(robot.buildCost * refundPercent));
        GameManager.Instance.AddCoins(refund);

        if (breakFxPrefab) Instantiate(breakFxPrefab, robot.transform.position, Quaternion.identity);
        if (pickaxeSfx) AudioSource.PlayClipAtPoint(pickaxeSfx, cam.transform.position, pickaxeVolume);

        if (robot.container != null)
        {
            robot.container.isFull = false;
            robot.container.Highlight(false);
        }

        Destroy(robot.gameObject);
    }

    void RemoveDefense(DefenseController defense)
    {
        int refund = Mathf.Max(0, Mathf.RoundToInt(defense.buildCost * refundPercent));
        GameManager.Instance.AddCoins(refund);

        if (breakFxPrefab) Instantiate(breakFxPrefab, defense.transform.position, Quaternion.identity);
        if (pickaxeSfx) AudioSource.PlayClipAtPoint(pickaxeSfx, cam.transform.position, pickaxeVolume);

        if (defense.container != null)
        {
            defense.container.isFull = false;
            defense.container.Highlight(false);
        }

        Destroy(defense.gameObject);
    }
}
