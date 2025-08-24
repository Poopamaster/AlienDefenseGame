using UnityEngine;

public class RobotShooter : MonoBehaviour
{
    [Header("Shoot Settings")]
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 8f;

    public void ShootEvent()
    {
        if (firePoint == null || bulletPrefab == null)
        {
            Debug.LogWarning("FirePoint หรือ BulletPrefab ยังไม่ได้ใส่ใน Inspector!");
            return;
        }

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.right * bulletSpeed; 
        }

        Debug.Log("ยิงกระสุนแล้ว!");
    }
}
