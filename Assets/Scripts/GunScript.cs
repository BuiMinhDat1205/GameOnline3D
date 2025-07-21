using UnityEngine;
using Fusion;

public class GunScript : NetworkBehaviour
{
    public NetworkPrefabRef bulletPrefabs; // D�ng NetworkPrefabRef ?? t??ng th�ch v?i Runner.Spawn
    public Transform firePoint;

    void Update()
    {
        if (Object.HasInputAuthority && Input.GetKeyDown(KeyCode.E))
        {
            if (Runner.IsServer || Runner.IsSharedModeMasterClient)
            {
                var bullet = Runner.Spawn(bulletPrefabs, firePoint.position, firePoint.rotation, Object.InputAuthority);
                bullet.GetComponent<BulletScript>().Owner = Object.InputAuthority;
                bullet.GetComponent<Rigidbody>().AddForce(firePoint.forward * 40f, ForceMode.Impulse);
            }
        }
    }
}
