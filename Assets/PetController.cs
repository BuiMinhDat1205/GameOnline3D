using UnityEngine;
using Fusion;

public class PetController : NetworkBehaviour
{
    // ... (các biến đã có)
    [Header("Pet Settings")]
    public float followDistance = 0.5f;
    public float moveSpeed = 3f;
    public float attackRange = 5f;
    public float fireRate = 1f;
    public Transform firePoint;
    public NetworkPrefabRef bulletPrefab;
    public float verticalOffset = 0.5f;
    public float horizontalOffset = 0.5f;

    private Transform targetPlayer;
    private float lastFireTime = 0f;
    private GameObject nearestEnemy;

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                targetPlayer = player.transform;
            }
        }
    }

    void Update()
    {
        if (targetPlayer == null) return;

        // --- Follow Player ---
        Vector3 playerForward = targetPlayer.forward;
        Vector3 playerRight = targetPlayer.right;

        Vector3 followPos = targetPlayer.position - playerForward * followDistance + playerRight * horizontalOffset;
        followPos.y = targetPlayer.position.y + verticalOffset;
        transform.position = Vector3.Lerp(transform.position, followPos, moveSpeed * Time.deltaTime);

        // --- Tìm và tấn công enemy trong phạm vi ---
        nearestEnemy = FindEnemyInAttackRange(); // Sửa từ đây
        if (nearestEnemy != null)
        {
            // Quay mặt về phía enemy
            Vector3 lookAtTarget = new Vector3(nearestEnemy.transform.position.x, transform.position.y, nearestEnemy.transform.position.z);
            transform.LookAt(lookAtTarget);

            if (Time.time - lastFireTime >= fireRate)
            {
                lastFireTime = Time.time;
                RPC_Attack(nearestEnemy.transform.position);
            }
        }
        else
        {
            // Nếu không có enemy trong tầm, quay mặt theo player
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(playerForward), 10f * Time.deltaTime);
        }
    }

    // Hàm mới để tìm kẻ địch trong phạm vi tấn công
    GameObject FindEnemyInAttackRange()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);
        GameObject nearest = null;
        float minDist = Mathf.Infinity;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                float dist = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = hitCollider.gameObject;
                }
            }
        }
        return nearest;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_Attack(Vector3 targetPos)
    {
        if (bulletPrefab.Equals(default)) return;

        Quaternion rot = Quaternion.LookRotation((targetPos - firePoint.position).normalized);
        var bullet = Runner.Spawn(bulletPrefab, firePoint.position, rot, Object.InputAuthority);

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce((targetPos - firePoint.position).normalized * 30f, ForceMode.Impulse);
        }
    }
}