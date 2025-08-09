using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class NetworkHealthEnemy : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnHealthChanged))]
    public float Health { get; set; }
    [Header("Enemy Health Settings")]
    // Biến để lưu máu của enemy
    public float MaxHealth = 100f;

    [Header("UI Health Bar")]
    // Biến để lưu thanh máu UI
    public Slider healthSlider; // slider gắn trên enemy
    public Transform uiCanvas;  // canvas world space trên đầu enemy
    // Biến để lưu camera chính
    [Header("Camera")]
    private Camera mainCam;

    public override void Spawned()
    {
        mainCam = Camera.main;
        Health = MaxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = MaxHealth;
            healthSlider.value = MaxHealth;
        }
    }

    void Update()
    {
        // Quay thanh máu về phía camera
        if (uiCanvas != null && mainCam != null)
        {
            // Đặt vị trí thanh máu trên đầu enemy
            uiCanvas.LookAt(mainCam.transform);
            uiCanvas.forward = mainCam.transform.forward;
        }
    }

    // Gọi khi máu thay đổi
    private void OnHealthChanged()
    {
        if (healthSlider != null)
        {
            healthSlider.value = Health;
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
        Runner.Despawn(Object); // Xóa khỏi network
    }

}
