using UnityEngine;
using Fusion;

public class BulletScript : NetworkBehaviour
{
    public float lifetime = 3f;
    public float speed = 15f;

    [Networked]
    public PlayerRef Owner { get; set; }

    private float timer = 0f;

    void Update()
    {
        if (Object.HasStateAuthority)
        {
            timer += Time.deltaTime;
            if (timer >= lifetime)
            {
                Runner.Despawn(Object);
            }
        }

        // Di chuyển đạn
        transform.position += transform.forward * Time.deltaTime * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!Object.HasStateAuthority) return;

        Debug.Log($"Bullet hit: {other.name}");

        bool didDamage = false;

        // Try to damage Player
        NetworkHealth playerHealth = other.GetComponent<NetworkHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(10);
            Debug.Log("Damaged player.");
            didDamage = true;
        }

        // Try to damage Enemy
        NetworkHealthEnemy enemyHealth = other.GetComponent<NetworkHealthEnemy>();
        if (enemyHealth != null)
        {
            enemyHealth.RPC_TakeDamage(10);
            Debug.Log("Damaged enemy.");
            didDamage = true;
        }

        if (!didDamage)
        {
            Debug.LogWarning($"No damageable component found on {other.name}. Tag: {other.tag}, Layer: {LayerMask.LayerToName(other.gameObject.layer)}");
        }

        // Despawn bullet
        Runner.Despawn(Object);
    }


}
