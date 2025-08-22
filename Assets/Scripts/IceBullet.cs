using UnityEngine;

public class IceBullet : MonoBehaviour
{
    public float movementSpeed = 12f;
    public int DamageValue = 10;

    [Header("Debuff")]
    [Tooltip("0.25 = ลด 25%")]
    public float slowPercentage = 0.25f;
    public float slowDuration = 2f;

    [Header("AoE")]
    [Tooltip("รัศมีตีหมู่เมื่อกระสุนชน")]
    public float aoeRadius = 1.5f;
    [Tooltip("เลเยอร์ของเอเลี่ยน (เช่น 9)")]
    public int alienLayer = 9;

    [Header("SFX")]
    public AudioClip hitSfx;
    [Range(0f, 1f)] public float hitVolume = 1f;

    void Update()
    {
        transform.Translate(new Vector3(movementSpeed, 0, 0));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == alienLayer)
        {
            DoAoeHit();

            if (hitSfx)
                AudioSource.PlayClipAtPoint(hitSfx, Camera.main.transform.position, hitVolume);

            Destroy(gameObject);
        }
    }

    private void DoAoeHit()
    {
        int mask = 1 << alienLayer;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, aoeRadius, mask);
        foreach (var h in hits)
        {
            var alien = h.GetComponent<AlienController>();
            if (alien == null) continue;

            alien.ReceiveDamage(DamageValue);
            alien.ApplySlow(slowPercentage, slowDuration);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }
}
