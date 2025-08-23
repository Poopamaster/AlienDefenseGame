using UnityEngine;
using UnityEngine.UI;

public class UIExplosion : MonoBehaviour
{
    public float duration = 0.45f;          // ความยาวเอฟเฟกต์
    public float startScale = 0.2f;         // เริ่มเล็ก
    public float endScale = 1.6f;           // ขยายใหญ่
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private RectTransform rect;
    private Image img;
    private float t;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        img  = GetComponent<Image>();
        if (img) img.raycastTarget = false; // ไม่บังการกด UI อื่น
        if (rect) rect.localScale = Vector3.one * startScale;
    }

    void Update()
    {
        if (!rect || !img) { Destroy(gameObject); return; }

        t += Time.deltaTime;
        float k = Mathf.Clamp01(t / duration);

        // scale
        float s = Mathf.Lerp(startScale, endScale, scaleCurve.Evaluate(k));
        rect.localScale = Vector3.one * s;

        // fade
        var c = img.color;
        c.a = 1f - k;
        img.color = c;

        if (k >= 1f) Destroy(gameObject);
    }
}
