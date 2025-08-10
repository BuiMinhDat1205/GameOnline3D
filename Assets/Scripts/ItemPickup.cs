using Fusion;
using UnityEngine;

public enum ItemType
{
    Health,
    Shield,
    Mana,
}

public class ItemPickup : NetworkBehaviour
{
    [Header("Item Settings")]
    public ItemType itemType;
    public float amount = 20f;

    private void OnTriggerEnter(Collider other)
    {
        var playerHealth = other.gameObject.GetComponent<NetworkHealth>();
        if (playerHealth == null) return;
        if (!playerHealth.Object.HasStateAuthority) return;

        switch (itemType)
        {
            case ItemType.Health:
                playerHealth.AddHealth(amount);
                break;

            case ItemType.Shield:
                playerHealth.AddShield(amount);
                break;

            case ItemType.Mana:
                var playerMove = other.gameObject.GetComponent<PlayerMovement>();
                if (playerMove != null)
                {
                    playerMove.AddMana(amount);           // Cần có hàm AddMana trong PlayerMovement (mình sẽ nói bên dưới)
                    playerMove.ActivateManaProtection(); // Kích hoạt trạng thái không giảm mana
                }
                else
                {
                    Debug.LogWarning("PlayerMovement component not found for Mana pickup.");
                }
                break;
            default:
                Debug.LogWarning($"Unknown item type: {itemType}");
                break;

        }

        Runner.Despawn(Object);
    }
}
