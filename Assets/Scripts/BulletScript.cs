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
        // Gây sát thương cho Player
        NetworkHealth playerHealth = other.GetComponent<NetworkHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(10); // dùng TakeDamage để tính giáp trước
        }

        // Gây sát thương cho Enemy
        NetworkHealthEnemy enemyHealth = other.GetComponent<NetworkHealthEnemy>();
        if (enemyHealth != null)
        {
            enemyHealth.RPC_TakeDamage(10);
        }

        // Hủy đạn sau khi va chạm
        Runner.Despawn(Object);
    }
}
