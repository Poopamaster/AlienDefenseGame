using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    public GameObject bullet;
    public List<GameObject> aliens;
    public GameObject toAttack;
    public float attackCooldown;
    public float attackTime;
    public int DamageValue;
    public int Health;
    internal ObjectContainer container;

    [Header("Economy")]
    public int buildCost = 0;

    private void Update()
    {
        toAttack = GetClosestAlienAhead();

        if (toAttack == null) return;

        if (Time.time >= attackTime)
        {
            GameObject bulletInstance = Instantiate(bullet, transform);

            var b = bulletInstance.GetComponent<Bullet>();
            if (b != null)
            {
                b.DamageValue = DamageValue;
            }
            else
            {
                var ice = bulletInstance.GetComponent<IceBullet>();
                if (ice != null) ice.DamageValue = DamageValue;
                else Debug.LogWarning("Bullet prefab ไม่มีคอมโพเนนต์ Bullet/IceBullet", bulletInstance);
            }

            attackTime = Time.time + attackCooldown;
        }
    }

    public void ReceiveDamage(int Damage)
    {
        Health -= Damage;
        if (Health <= 0)
        {
            if (container != null)
            {
                container.isFull = false;
                container.Highlight(false);
            }
            Destroy(gameObject);
        }
    }

    private GameObject GetClosestAlienAhead()
    {
        if (aliens == null || aliens.Count == 0) return null;

        float myX = transform.position.x;
        GameObject best = null;
        float bestDist = float.PositiveInfinity;

        for (int i = aliens.Count - 1; i >= 0; i--)
        {
            var a = aliens[i];
            if (a == null || !a.activeInHierarchy) { aliens.RemoveAt(i); continue; }

            float ax = a.transform.position.x;

            if (ax <= myX) continue;

            float dist = ax - myX;
            if (dist < bestDist)
            {
                bestDist = dist;
                best = a;
            }
        }

        return best;
    }
}
