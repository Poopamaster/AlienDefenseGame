using UnityEngine;

public class DefenseController : MonoBehaviour
{
    [Header("Stats")]
    public int Health;

    [Header("Economy")]
    public int buildCost = 0;

    [HideInInspector] public ObjectContainer container;

    public void ReceiveDamage(int damage)
    {
        Health -= Mathf.Max(0, damage);
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
