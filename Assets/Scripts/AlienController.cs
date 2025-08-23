using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AlienController : MonoBehaviour
{
    public int Health;
    public float DamageCooldown;
    public float Speed;
    private float originalSpeed;
    private bool isStopped;
    public int coinReward;
    [SerializeField] private string goalTag = "Goal";

    [Header("SFX")]
    public AudioClip slashSfx;
    [Range(0f, 1f)] public float slashVolume = 1f;
    private Coroutine slowRoutine;

    private Image[] uiImages;
    private Color[] originalColors;

    private float slowEndTime = 0f;

    [Header("Visual")]
    [Tooltip("สีตอนโดนสโลว์จากกระสุนน้ำแข็ง")]
    public Color icedColor = new Color(0.2f, 0.7f, 1f, 1f);

    [Header("Attack")]
    public int DamageValue = 10;
    public int BaseDamage = 0;

    void Start()
    {
        originalSpeed = Speed;

        if (BaseDamage <= 0) BaseDamage = DamageValue;

        uiImages = GetComponentsInChildren<Image>(includeInactive: false);
        if (uiImages != null && uiImages.Length > 0)
        {
            originalColors = new Color[uiImages.Length];
            for (int i = 0; i < uiImages.Length; i++)
                originalColors[i] = uiImages[i].color;
        }
    }

    void Update()
    {
        if (!isStopped)
        {
            // transform.Translate(new Vector3(Speed * -1, 0, 0));
            transform.Translate(Vector3.left * Speed * Time.deltaTime);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(goalTag))
        {
            GameManager.Instance.GameOver();
            return;
        }
        if (collision.gameObject.layer == 8)
        {
            StartCoroutine(Attack(collision));
            isStopped = true;
        }
    }

    IEnumerator Attack(Collider2D collision)
    {
        if (collision == null) { isStopped = false; yield break; }

        while (true)
        {
            if (collision == null || collision.gameObject == null)
                break;

            var rc = collision.gameObject.GetComponent<RobotController>();
            var dc = collision.gameObject.GetComponent<DefenseController>();

            if (rc == null && dc == null)
                break;

            if (rc != null) rc.ReceiveDamage(DamageValue);
            else            dc.ReceiveDamage(DamageValue);

            if (slashSfx)
                AudioSource.PlayClipAtPoint(slashSfx, Camera.main.transform.position, slashVolume);

            float t = 0f;
            while (t < DamageCooldown)
            {
                if (collision == null || collision.gameObject == null) break;
                t += Time.deltaTime;
                yield return null;
            }

            if (collision == null || collision.gameObject == null) break;
        }

        isStopped = false;
    }

    public void ReceiveDamage(int Damage)
    {
        if (Health - Damage <= 0)
        {
            GameManager.Instance.AddCoins(coinReward);
            transform.parent.GetComponent<SpawnPoint>().aliens.Remove(this.gameObject);
            Destroy(this.gameObject);
        }
        else
        {
            Health -= Damage;
        }
    }

    public void ApplySlow(float slowPercent, float duration)
    {
        slowPercent = Mathf.Clamp01(slowPercent);

        Speed = originalSpeed * (1f - slowPercent);

        if (uiImages != null)
        {
            for (int i = 0; i < uiImages.Length; i++)
                if (uiImages[i] != null) uiImages[i].color = icedColor;
        }

        slowEndTime = Mathf.Max(slowEndTime, Time.time + duration);

        if (slowRoutine != null) StopCoroutine(slowRoutine);
        slowRoutine = StartCoroutine(RemoveSlowWhenExpired());
    }

    private IEnumerator RemoveSlowWhenExpired()
    {
        while (Time.time < slowEndTime)
            yield return null;

        Speed = originalSpeed;

        if (uiImages != null && originalColors != null)
        {
            for (int i = 0; i < uiImages.Length; i++)
                if (uiImages[i] != null) uiImages[i].color = originalColors[i];
        }

        slowRoutine = null;
    }

    public void ScaleDamage(float multiplier)
    {
        multiplier = Mathf.Max(0f, multiplier);
        DamageValue = Mathf.CeilToInt(BaseDamage * multiplier);
    }
}
