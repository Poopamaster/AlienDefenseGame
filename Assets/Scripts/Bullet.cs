using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float movementSpeed;
    public int DamageValue;

    [Header("SFX")]
    public AudioClip hitSfx;
    [Range(0f, 1f)] public float hitVolume = 1f;
    void Update()
    {
        transform.Translate(Vector3.right * movementSpeed * Time.deltaTime);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            collision.gameObject.GetComponent<AlienController>().ReceiveDamage(DamageValue);
            if (hitSfx)
            {
                AudioSource.PlayClipAtPoint(hitSfx, Camera.main.transform.position, hitVolume);
            }
            Destroy(this.gameObject);
        }
    }

}
