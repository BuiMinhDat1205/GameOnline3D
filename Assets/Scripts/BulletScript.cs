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
        // Tăng thời gian sống
        if (Object.HasStateAuthority)
        {
            timer += Time.deltaTime;

            if (timer >= lifetime)
            {
                Runner.Despawn(Object);
            }
        }
        transform.position += transform.forward * Time.deltaTime * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!Object.HasStateAuthority) return;

        // Nếu va vào chính player đã bắn thì bỏ qua
        NetworkObject netObj = other.GetComponent<NetworkObject>();
        if (netObj != null && netObj.InputAuthority == Owner)
        {
            return;
        }

        // Gây sát thương cho player
        NetworkHealth playerHealth = other.GetComponent<NetworkHealth>();
        if (playerHealth != null)
        {
            playerHealth.TruHealth(10); // Gây 10 damage
        }

        // Gây sát thương cho enemy
        NetworkHealthEnemy enemyHealth = other.GetComponent<NetworkHealthEnemy>();
        if (enemyHealth != null)
        {
            enemyHealth.RPC_TakeDamage(10); // Gây 10 damage
        }

        // Hủy viên đạn
        Runner.Despawn(Object);
    }

}
