using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class NetworkHealthEnemy : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnHealthChanged))]
    public float Health { get; set; }

    [Header("Enemy Health Settings")]
    private float MaxHealth = 100f;

    [Header("UI Health Bar")]
    public Slider healthBar;
    public Transform uiCanvas;

    [Header("Camera")]
    private Camera mainCam;

    [System.Serializable]
    public class DropItem
    {
        public GameObject prefab;
        [Range(0f, 1f)] public float dropChance = 0.5f; // 0.5 = 50%
    }

    [Header("Item Drop Settings")]
    public DropItem[] dropItems; // Kéo nhiều item vào đây

    public override void Spawned()
    {
        mainCam = Camera.main;
        Health = MaxHealth;

        if (healthBar != null)
        {
            healthBar.maxValue = MaxHealth;
            healthBar.value = MaxHealth;
        }
    }

    void Update()
    {
        if (uiCanvas != null && mainCam != null)
        {
            uiCanvas.LookAt(mainCam.transform);
            uiCanvas.forward = mainCam.transform.forward;
        }
    }

    private void OnHealthChanged()
    {
        if (healthBar != null)
        {
            healthBar.value = Health;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_TakeDamage(float damage)
    {
        if (Health <= 0) return;

        Health -= damage;
        if (Health <= 0)
        {
            Health = 0;
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy chết!");
        DropItems();
        Runner.Despawn(Object);
    }

    void DropItems()
    {
        foreach (var item in dropItems)
        {
            if (item.prefab != null && Random.value <= item.dropChance)
            {
                Runner.Spawn(item.prefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
            }
        }
    }
}
