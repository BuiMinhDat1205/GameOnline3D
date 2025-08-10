using Fusion;
using UnityEngine;

public enum ItemType
{
    Health,
    Shield,
    Ammo
}

public class ItemPickup : NetworkBehaviour
{
    [Header("Item Settings")]
    public ItemType itemType;
    public float amount = 20f;

    private void OnCollisionEnter(Collider other)
    {
        // Kiểm tra player
        var playerHealth = other.GetComponent<NetworkHealth>();
        if (playerHealth == null) return;

        // Chỉ StateAuthority mới xử lý logic
        if (!playerHealth.Object.HasStateAuthority) return;

        switch (itemType)
        {
            case ItemType.Health:
                playerHealth.AddHealth(amount);
                break;
            case ItemType.Shield:
                playerHealth.AddShield(amount);
                break;
        }

        // Hủy item sau khi nhặt
        Runner.Despawn(Object);
    }
}
