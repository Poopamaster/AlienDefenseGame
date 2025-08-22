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

        if (Input.GetMouseButtonDown(1))
        {
            GameManager.Instance.SetPickaxeMode(false);
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            TryPickaxeUI();
        }
    }

    void TryPickaxeUI()
    {
        if (es == null || raycasters == null || raycasters.Length == 0) return;

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

                var robot = r.gameObject.GetComponent<RobotController>() ??
                            r.gameObject.GetComponentInParent<RobotController>();
                if (robot != null)
                {
                    RemoveRobot(robot);
                    return;
                }
            }
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
}
