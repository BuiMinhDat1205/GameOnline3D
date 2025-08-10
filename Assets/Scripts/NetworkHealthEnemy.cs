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
    public Slider healthBar; // slider gắn trên enemy
    public Transform uiCanvas;  // canvas world space trên đầu enemy

    [Header("Camera")]
    private Camera mainCam;

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
        // Quay thanh máu về phía camera
        if (uiCanvas != null && mainCam != null)
        {
            uiCanvas.LookAt(mainCam.transform);
            uiCanvas.forward = mainCam.transform.forward;
        }
        // ❌ Bỏ dòng reset healthBar.value = MaxHealth;
        // Vì giá trị máu sẽ được cập nhật qua OnHealthChanged()
    }

    // Gọi khi máu thay đổi
    private void OnHealthChanged()
    {
        if (healthBar != null)
        {
            healthBar.value = Health;
        }
    }

    // Hàm nhận sát thương
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
        Runner.Despawn(Object);
    }
}
