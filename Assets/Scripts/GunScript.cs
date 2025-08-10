using UnityEngine;
using Fusion;
using TMPro;

public class GunScript : NetworkBehaviour
{
    public NetworkPrefabRef bulletPrefabs;
    public Transform firePoint;
    public Animator anim;

    [SerializeField] private TextMeshProUGUI ammoText;

    [Networked]
    public int currentAmmo { get; set; } // Số đạn trong băng hiện tại

    [Networked]
    public int maxAmmo { get; set; }     // Tổng đạn dự trữ (ngoài băng)

    private int clipSize = 25;           // Kích thước băng đạn

    private int lastAmmo = -1;

    private void Awake()
    {
        if (anim == null)
            anim = GetComponentInParent<Animator>();
    }

    public override void Spawned()
    {
        currentAmmo = clipSize;  // Khởi đầu đầy băng
        maxAmmo = 150;           // Tổng đạn dự trữ

        UpdateAmmoUI();
    }

    public override void Render()
    {
        base.Render();

        if (lastAmmo != currentAmmo || lastMaxAmmo != maxAmmo)
        {
            lastAmmo = currentAmmo;
            lastMaxAmmo = maxAmmo;
            UpdateAmmoUI();
        }
    }

    private int lastMaxAmmo = -1;

    void Update()
    {
        if (!Object.HasInputAuthority)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentAmmo > 0)
            {
                RPC_Fire();
            }
            else
            {
                Debug.Log("Out of ammo! Press R to reload.");
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            RPC_Reload();
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_Fire()
    {
        if (currentAmmo <= 0)
            return;

        SpawnBullet();
        currentAmmo--;
        RPC_PlayShootAnim();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_PlayShootAnim()
    {
        if (anim != null)
        {
            anim.ResetTrigger("Shoot");
            anim.SetTrigger("Shoot");
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_Reload()
    {
        if (maxAmmo <= 0)
            return; // Không còn đạn dự trữ reload

        int neededAmmo = clipSize - currentAmmo; // Đạn cần nạp để đầy băng

        int ammoToLoad = Mathf.Min(neededAmmo, maxAmmo);

        currentAmmo += ammoToLoad;
        maxAmmo -= ammoToLoad;

        RPC_PlayReloadAnim();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_PlayReloadAnim()
    {
        if (anim != null)
        {
            anim.ResetTrigger("Reload");
            anim.SetTrigger("Reload");
        }
    }

    private void SpawnBullet()
    {
        Quaternion direction = Quaternion.LookRotation(transform.forward);
        Quaternion correction = Quaternion.Euler(-90, 180, 0);
        Quaternion finalRotation = direction * correction;

        var bullet = Runner.Spawn(bulletPrefabs, firePoint.position, finalRotation, Object.InputAuthority);
        bullet.GetComponent<BulletScript>().Owner = Object.InputAuthority;
        bullet.GetComponent<Rigidbody>().AddForce(firePoint.forward * 40f, ForceMode.Impulse);
    }

    private void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = $"{currentAmmo} | {maxAmmo}";
        }
    }

}
