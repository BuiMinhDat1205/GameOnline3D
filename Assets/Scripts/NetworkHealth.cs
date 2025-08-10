using Fusion;
using System;
using UnityEngine;
using UnityEngine.UI;

public class NetworkHealth : NetworkBehaviour
{
    // Máu
    [Networked, OnChangedRender(nameof(OnHealthChanged))]
    public float Health { get; set; }

    // Giáp
    [Networked, OnChangedRender(nameof(OnShieldChanged))]
    public float Shield { get; set; }

    [Header("Máu")]
    public GameObject healthNetwork;
    [SerializeField] Slider healthSlider;
    public GameObject healthLocal;
    [SerializeField] Slider healthSliderLocal;

    [Header("Giáp")]
    public GameObject shieldNetwork;
    [SerializeField] Slider shieldSlider;
    public GameObject shieldLocal;
    [SerializeField] Slider shieldSliderLocal;

    [Header("Chỉ số tối đa")]
    public float maxHealth = 100f;
    public float maxShield = 50f;

    private void Start()
    {
        // Khởi tạo
        if (Object.HasStateAuthority)
        {
            Health = maxHealth;
            Shield = 0; // ban đầu chưa có giáp
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        // Cập nhật UI local hay network
        bool isLocal = Object.HasInputAuthority;

        healthLocal.SetActive(isLocal);
        healthNetwork.SetActive(!isLocal);

        shieldLocal.SetActive(isLocal);
        shieldNetwork.SetActive(!isLocal);

        // Thanh máu
        healthSlider.value = Health;
        healthSliderLocal.value = Health;

        // Thanh giáp
        shieldSlider.value = Shield;
        shieldSliderLocal.value = Shield;
    }

    public void OnHealthChanged()
    {
        UpdateUI();
    }

    public void OnShieldChanged()
    {
        UpdateUI();
    }

    // Hàm nhận sát thương
    public void TakeDamage(float damage)
    {
        if (!Object.HasStateAuthority) return;

        if (Shield > 0)
        {
            float damageToShield = Mathf.Min(damage, Shield);
            Shield -= damageToShield;
            damage -= damageToShield;
        }

        if (damage > 0)
        {
            Health -= damage;
        }

        if (Health <= 0)
        {
            Health = 0;
            Debug.Log("Player chết");
            // TODO: Xử lý chết
        }
    }

    public void AddShield(float amount)
    {
        if (!Object.HasStateAuthority) return;
        Shield = Mathf.Clamp(Shield + amount, 0, maxShield);
    }

    public float GetHp()
    {
        return Health;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!Object.HasStateAuthority) return;

        if (hit.gameObject.CompareTag("Bullet"))
        {
            TakeDamage(10);
            Debug.Log("Bị bắn");
        }
        else if (hit.gameObject.CompareTag("ShieldItem"))
        {
            AddShield(20);
            Destroy(hit.gameObject);
            Debug.Log("Nhặt khiên");
        }
        else if (hit.gameObject.CompareTag("HealthItem"))
        {
            Health = Mathf.Clamp(Health + 20, 0, maxHealth);
            Destroy(hit.gameObject);
            Debug.Log("Nhặt máu");
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_Heal(float amount)
    {
        Health = Mathf.Min(Health + amount, Health);
    }

    internal void AddHealth(float value)
    {
        if (!Object.HasStateAuthority) return;
        Health = Mathf.Clamp(Health + value, 0, maxHealth);
    }
}
