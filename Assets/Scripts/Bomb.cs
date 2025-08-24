using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("AOE")]
    [Tooltip("รัศมีระเบิด (world units). ถ้า 1 ช่อง ~150 หน่วย แนะนำเริ่ม 200–300")]
    public float radius = 250f;

    [Tooltip("ความเสียหาย (ตั้งสูงๆ = กวาดทั้งวง)")]
    public int damage = 99999;

    [Header("Arming")]
    [Tooltip("หน่วงก่อนเริ่มทำงาน (กันวางแล้วระเบิดทันที)")]
    public float armTime = 0.25f;

    [Tooltip("ถ้าไม่มีเอเลี่ยนเข้ามาในรัศมี จะระเบิดอัตโนมัติหลังเวลานี้ (0 = ไม่ใช้)")]
    public float autoExplodeAfter = 5f;

    [Header("Fallback (ไม่จำเป็นถ้าใช้งานลิสต์เลน)")]
    public LayerMask alienLayer;

    [Header("FX (UI)")]
    public UIExplosion uiExplodePrefab;
    public Vector2 uiExplodeSize = new Vector2(220, 220);

    [Header("Audio")]
    public AudioClip explodeSfx;
    [Range(0f, 1f)] public float explodeVolume = 1f;

    [HideInInspector] public ObjectContainer container;
    private List<GameObject> aliens;

    private float spawnTime;
    private bool armed = false;
    private bool exploded = false;

    void Start()
    {
        spawnTime = Time.time;

        if (container != null && container.spawnPoint != null)
        {
            aliens = container.spawnPoint.aliens;
        }
    }

    void Update()
    {
        if (exploded) return;

        if (!armed)
        {
            if (Time.time - spawnTime >= armTime) armed = true;
            else return;
        }

        if (aliens != null && aliens.Count > 0)
        {
            if (AnyAlienInRangeFromList()) { Explode(); return; }
        }
        else
        {
            if (AnyAlienInRangePhysics())  { Explode(); return; }
        }

        if (autoExplodeAfter > 0f && Time.time - spawnTime >= autoExplodeAfter)
        {
            Explode();
        }
    }

    bool AnyAlienInRangeFromList()
    {
        for (int i = aliens.Count - 1; i >= 0; i--)
        {
            var a = aliens[i];
            if (a == null || !a.activeInHierarchy) { aliens.RemoveAt(i); continue; }

            if (Vector3.Distance(transform.position, a.transform.position) <= radius)
                return true;
        }
        return false;
    }

    bool AnyAlienInRangePhysics()
    {
        var cols = Physics2D.OverlapCircleAll(transform.position, radius, alienLayer);
        return cols != null && cols.Length > 0;
    }

void Explode()
{
    if (exploded) return;
    exploded = true;

    Vector3 origin;
    var rt = GetComponent<RectTransform>();
    origin = (rt != null && container != null) ? container.transform.position : transform.position;

    Collider2D[] cols = Physics2D.OverlapCircleAll(origin, radius, alienLayer);
    if (cols == null || cols.Length == 0)
        cols = Physics2D.OverlapCircleAll(origin, radius);

    var targets = new HashSet<AlienController>();
    foreach (var c in cols)
    {
        if (c == null) continue;
        var ac = c.GetComponentInParent<AlienController>() ??
                 c.GetComponent<AlienController>() ??
                 c.GetComponentInChildren<AlienController>();
        if (ac != null && ac.gameObject.activeInHierarchy)
            targets.Add(ac);
    }

    foreach (var ac in targets)
    {
        if (ac == null) continue;

        if (ac.Health - damage <= 0)
        {
            int reduced = Mathf.RoundToInt(ac.coinReward * 0.05f);
            GameManager.Instance.AddCoins(reduced);

            var sp = ac.transform.parent ? ac.transform.parent.GetComponent<SpawnPoint>() : null;
            if (sp != null) sp.aliens.Remove(ac.gameObject);

            Destroy(ac.gameObject);
        }
        else
        {
            ac.ReceiveDamage(damage);
        }
    }

    if (rt != null && uiExplodePrefab != null)
    {
        var fx = Instantiate(uiExplodePrefab, rt.parent);
        var fxRT = fx.GetComponent<RectTransform>();
        fxRT.anchorMin = fxRT.anchorMax = new Vector2(0.5f, 0.5f);
        fxRT.anchoredPosition = rt.anchoredPosition;
        fxRT.sizeDelta = uiExplodeSize;
        fxRT.localRotation = Quaternion.identity;
        fxRT.localScale = Vector3.one;
    }

    if (explodeSfx) AudioSource.PlayClipAtPoint(explodeSfx, Camera.main.transform.position, explodeVolume);

    if (container != null)
    {
        container.isFull = false;
        container.Highlight(false);
    }

    Destroy(gameObject);
}



    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
