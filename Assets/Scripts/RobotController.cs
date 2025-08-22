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
        if (aliens.Count > 0)
        {
            float distance = 1900;
            foreach (GameObject alien in aliens)
            {
                float alienDistance = Vector3.Distance(transform.position, alien.transform.position);
                if (alienDistance < distance)
                {
                    toAttack = alien;
                    distance = alienDistance;
                }
            }
        }
        else
        {
            toAttack = null;
        }
        if (toAttack != null)
        {
            if (attackTime <= Time.time)
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
                    if (ice != null)
                    {
                        ice.DamageValue = DamageValue;
                    }
                    else
                    {
                        Debug.LogWarning("Bullet prefab ไม่มีคอมโพเนนต์ Bullet/IceBullet", bulletInstance);
                    }
                }

                attackTime = Time.time + attackCooldown;
            }
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
}
