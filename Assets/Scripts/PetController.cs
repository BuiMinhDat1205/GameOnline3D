using UnityEngine;
using Fusion;
using System.Collections;

public class PetController : NetworkBehaviour
{
    [Header("Pet Settings")]
    public float orbitRadius = 2f;         // Bán kính bay quanh player
    public float orbitSpeed = 50f;         // Tốc độ quay vòng
    public float moveSmoothness = 5f;      // Độ mượt khi di chuyển
    public float attackRange = 15f;        // Tầm bắn enemy
    public float fireRate = 1f;            // Tốc độ bắn
    public float fightDistance = 5f;       // Khoảng cách đứng bắn với enemy
    public Transform firePoint;
    public NetworkPrefabRef bulletPrefab;

    private Transform targetPlayer;
    private float lastFireTime = 0f;
    private GameObject nearestEnemy;
    private float orbitAngle = 0f;
    private bool isReturning = false;
    private bool isWaiting = false;

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
        if (isWaiting) return;

        nearestEnemy = FindEnemyInAttackRange();

        if (nearestEnemy != null && !isReturning)
        {
            // --- Di chuyển tới vị trí chiến đấu gần enemy ---
            Vector3 fightPos = nearestEnemy.transform.position
                               + (transform.position - nearestEnemy.transform.position).normalized * fightDistance;
            fightPos.y = targetPlayer.position.y + 0.5f;

            transform.position = Vector3.Lerp(transform.position, fightPos, moveSmoothness * Time.deltaTime);

            // Quay mặt về enemy
            Vector3 lookAtTarget = new Vector3(nearestEnemy.transform.position.x,
                                               transform.position.y,
                                               nearestEnemy.transform.position.z);
            transform.LookAt(lookAtTarget);

            // Tấn công
            if (Time.time - lastFireTime >= fireRate)
            {
                lastFireTime = Time.time;
                RPC_Attack(nearestEnemy.transform.position);
            }

            // Nếu enemy biến mất hoặc chết thì quay lại player
            if (!nearestEnemy.activeInHierarchy)
            {
                StartCoroutine(ReturnToPlayer());
            }
        }
        else if (!isReturning)
        {
            // --- Bay vòng quanh player ---
            orbitAngle += orbitSpeed * Time.deltaTime;
            float rad = orbitAngle * Mathf.Deg2Rad;

            Vector3 offset = new Vector3(Mathf.Cos(rad), 0.5f, Mathf.Sin(rad)) * orbitRadius;
            Vector3 targetPos = targetPlayer.position + offset;

            transform.position = Vector3.Lerp(transform.position, targetPos, moveSmoothness * Time.deltaTime);

            // Quay mặt theo hướng di chuyển
            Vector3 dir = (targetPos - transform.position).normalized;
            if (dir.sqrMagnitude > 0.01f)
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 10f * Time.deltaTime);
        }
    }

    // --- Coroutine: bay về player khi giết quái ---
    private IEnumerator ReturnToPlayer()
    {
        isReturning = true;

        while (Vector3.Distance(transform.position, targetPlayer.position) > 0.5f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPlayer.position, 6f * Time.deltaTime);
            yield return null;
        }

        // Dừng 2s rồi quay lại bay vòng
        isWaiting = true;
        yield return new WaitForSeconds(2f);
        isWaiting = false;
        isReturning = false;
    }

    // --- Tìm Enemy gần nhất trong tầm ---
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

        // Ngắm vào ngang người enemy
        Vector3 aimTarget = targetPos + Vector3.up * 1.2f; 
        Vector3 dir = (aimTarget - firePoint.position).normalized; 
        //Hiệu chỉnh hướng bắn
        Quaternion direction = Quaternion.LookRotation(transform.forward);
        Quaternion correction = Quaternion.Euler(-90, 180, 0);
        Quaternion rot = Quaternion.LookRotation(dir) * correction;
        var bullet = Runner.Spawn(bulletPrefab, firePoint.position, rot, Object.InputAuthority);

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = dir * 30f;
        }
    }
}
