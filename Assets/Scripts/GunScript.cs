using UnityEngine;
using Fusion;

public class GunScript : NetworkBehaviour
{
    public NetworkPrefabRef bulletPrefabs; // Dùng NetworkPrefabRef ?? t??ng thích v?i Runner.Spawn
    public Transform firePoint;

    void Update()
    {
        if (Object.HasInputAuthority && Input.GetKeyDown(KeyCode.E))
        {
            if (Runner.IsServer || Runner.IsSharedModeMasterClient)
            {
                Quaternion direction = Quaternion.LookRotation(transform.forward);
                Quaternion correction = Quaternion.Euler(-90, 180, 0); // Tùy chỉnh theo mô hình bạn
                Quaternion finalRotation = direction * correction;
                var bullet = Runner.Spawn(bulletPrefabs, firePoint.position, finalRotation, Object.InputAuthority);
                bullet.GetComponent<BulletScript>().Owner = Object.InputAuthority;
                bullet.GetComponent<Rigidbody>().AddForce(firePoint.forward * 40f, ForceMode.Impulse);
            }
        }
    }
}
