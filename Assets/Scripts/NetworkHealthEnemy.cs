using Fusion;
using System.Collections;
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

        // Đợi một frame trước khi despawn enemy để spawn item kịp xử lý
        StartCoroutine(DelayedDespawn());
    }

    private IEnumerator DelayedDespawn()
    {
        yield return null; // chờ 1 frame
        Runner.Despawn(Object);
    }

    void DropItems()
    {
        foreach (var item in dropItems)
        {
            if (item.prefab != null && Random.value <= item.dropChance)
            {
                Vector3 spawnPos = transform.position + Vector3.up * 5f;
                RaycastHit hit;
                if (Physics.Raycast(spawnPos, Vector3.down, out hit, 10f))
                {
                    spawnPos = hit.point + Vector3.up * 0.5f;
                }
                Runner.Spawn(item.prefab, spawnPos, Quaternion.identity);
            }
        }
    }
}
