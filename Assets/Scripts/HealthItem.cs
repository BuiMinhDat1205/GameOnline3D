using Fusion;
using UnityEngine;

public class HealthItem : NetworkBehaviour
{
    [Header("Health Settings")]
    public float healAmount = 25f; // Lượng máu hồi

    [Header("Destroy Settings")]
    public float respawnTime = 0f; // Nếu > 0 thì spawn lại, nếu = 0 thì biến mất luôn

    private void OnTriggerEnter(Collider other)
    {
        // Kiểm tra nếu là Player và có NetworkHealth
        NetworkHealth playerHealth = other.GetComponent<NetworkHealth>();
        if (playerHealth != null)
        {
            // Gọi RPC để hồi máu (RPC chạy trên StateAuthority)
            playerHealth.RPC_Heal(healAmount);

            // Xử lý biến mất vật phẩm
            if (respawnTime > 0)
            {
                // Nếu muốn respawn lại (có thể viết thêm code spawn lại ở đây)
                Runner.Despawn(Object);
                // Có thể sử dụng SpawnManager của bạn để tạo lại item sau thời gian delay
            }
            else
            {
                Runner.Despawn(Object);
            }
        }
    }
}
