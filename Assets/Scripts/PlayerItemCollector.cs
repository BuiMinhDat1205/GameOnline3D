using Fusion;
using UnityEngine;

public class PlayerItemCollector : NetworkBehaviour
{
    public NetworkHealth health; // Script máu của player
    public float coins = 0;

    public void ApplyItemEffect(ItemType type, float value)
    {
        switch (type)
        {
            case ItemType.Health:
                health.AddHealth(value);
                Debug.Log($"+{value} HP");
                break;

            case ItemType.Shield:
                health.AddShield(value);
                Debug.Log($"+{value} Shield");
                break;

        }
    }
}
