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

    private void OnCollisionEnter(Collision other)
    {
        // Use other.gameObject to access components on the collided object
        var playerHealth = other.gameObject.GetComponent<NetworkHealth>();
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
