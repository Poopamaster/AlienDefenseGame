using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienController : MonoBehaviour
{
    public int Health;
    public int DamageValue;
    public float DamageCooldown;
    public float Speed;
    private bool isStopped;
    public int coinReward;
    [SerializeField] private string goalTag = "Goal";

    [Header("SFX")]
    public AudioClip slashSfx;
    [Range(0f, 1f)] public float slashVolume = 1f;
    void Update()
    {
        if (!isStopped)
        {
            transform.Translate(new Vector3(Speed * -1, 0, 0));
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

        if (rc != null)
            rc.ReceiveDamage(DamageValue);
        else
            dc.ReceiveDamage(DamageValue);

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
}
