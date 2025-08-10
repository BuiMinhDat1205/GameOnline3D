using Fusion;
using UnityEngine;

public class ShieldItem : NetworkBehaviour
{
    public float shieldAmount = 20f; // Lượng giáp cộng thêm

    private void OnTriggerEnter(Collider other)
    {
        // Kiểm tra có phải player không
        NetworkHealth playerHealth = other.GetComponent<NetworkHealth>();
        if (playerHealth != null)
        {
            // Chỉ StateAuthority mới xử lý cộng giáp
            if (playerHealth.Object.HasStateAuthority)
            {
                playerHealth.AddShield(shieldAmount);
            }

            // Hủy item sau khi nhặt
            Runner.Despawn(Object);
        }
    }
}
