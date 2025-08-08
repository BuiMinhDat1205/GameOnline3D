using UnityEngine;
using Fusion;

public class GunScript : NetworkBehaviour
{
    public NetworkPrefabRef bulletPrefabs; 
    public Transform firePoint;
    public Animator anim; 

    private void Awake()
    {
        if (anim == null)
            anim = GetComponentInParent<Animator>();
    }

    void Update()
    {
        if (Object.HasInputAuthority && Input.GetKeyDown(KeyCode.E))
        {
            RPC_Fire();
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_Fire()
    {
        SpawnBullet();
        RPC_PlayShootAnim();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_PlayShootAnim()
    {
        if (anim != null)
        {
            anim.ResetTrigger("Shoot"); // reset để tránh trigger kẹt
            anim.SetTrigger("Shoot");
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
}
