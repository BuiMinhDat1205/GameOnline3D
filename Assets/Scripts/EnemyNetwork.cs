using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Fusion;

public class EnemyNetwork : NetworkBehaviour
{
    [Header("Target Player")]
    private GameObject[] players;

    [Header("NavMesh Agent")]
    public NavMeshAgent agent;

    [Header("Fire Settings")]
    public Transform firePoint;
    public GameObject firePrefab;
    public float fireCooldown = 1f;
    private float fireTimer = 0f;

    [Header("Distance Settings")]
    public float chaseDistance = 20f;
    public float attackDistance = 8f;
    private Vector3 startPosition;

    [Header("Animation")]
    public Animator animator; // Animator với Blend Tree (Speed) và bool IsShooting

    [Header("Dodge Movement")]
    public float dodgeDistance = 3f;         // Khoảng cách né
    public float dodgeChangeInterval = 1f;  // Thay đổi vị trí né sau từng giây

    private Vector3 dodgeTarget;
    private float dodgeTimer = 0f;
    private bool dodgeRight = true;

    private void Start()
    {
        if (agent != null)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(agent.transform.position, out hit, 2f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
                startPosition = hit.position;
            }
            else
            {
                Debug.LogWarning($"{name} spawn ngoài NavMesh! Tìm vị trí gần nhất không thành công.");
            }
        }
        else
        {
            Debug.LogError($"{name} không có NavMeshAgent được gán!");
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (agent == null || !agent.isOnNavMesh) return;

        players = GameObject.FindGameObjectsWithTag("Player");
        if (players == null || players.Length == 0)
        {
            UpdateAnimator(0f, false);
            return;
        }

        GameObject closest = FindClosestPlayer();
        if (closest == null) return;

        float dist = Vector3.Distance(closest.transform.position, transform.position);

        if (dist <= chaseDistance)
        {
            if (dist > attackDistance)
            {
                // Chase player
                if (agent.isStopped) agent.isStopped = false;
                agent.SetDestination(closest.transform.position);
                UpdateAnimator(agent.velocity.magnitude, false);
            }
            else
            {
                // Attack + dodge movement
                dodgeTimer += Runner.DeltaTime;

                if (dodgeTimer >= dodgeChangeInterval)
                {
                    dodgeRight = !dodgeRight; // đổi hướng né
                    dodgeTarget = CalculateDodgePosition(closest.transform.position, dodgeRight);
                    dodgeTimer = 0f;
                }

                if (!agent.isStopped) agent.isStopped = false;
                agent.SetDestination(dodgeTarget);

                transform.LookAt(new Vector3(closest.transform.position.x, transform.position.y, closest.transform.position.z));
                UpdateAnimator(agent.velocity.magnitude, true);

                // Fire logic
                fireTimer += Runner.DeltaTime;
                if (fireTimer >= fireCooldown)
                {
                    RpcFire();
                    fireTimer = 0f;
                }
            }
        }
        else
        {
            if (!agent.isStopped) agent.isStopped = false;
            agent.SetDestination(startPosition);
            UpdateAnimator(agent.velocity.magnitude, false);
        }
    }

    private void UpdateAnimator(float speed, bool isShooting)
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", speed);
            animator.SetBool("IsShooting", isShooting);
        }
    }

    private GameObject FindClosestPlayer()
    {
        if (players == null || players.Length == 0)
            return null;

        GameObject closest = null;
        float closestDist = Mathf.Infinity;

        foreach (var p in players)
        {
            if (p == null) continue;

            float dist = Vector3.Distance(p.transform.position, transform.position);
            if (dist < closestDist)
            {
                closest = p;
                closestDist = dist;
            }
        }

        return closest;
    }

    private Vector3 CalculateDodgePosition(Vector3 playerPos, bool toRight)
    {
        // Hướng từ enemy đến player
        Vector3 dirToPlayer = (playerPos - transform.position).normalized;

        // Hướng vuông góc sang phải hoặc trái
        Vector3 dodgeDir = toRight ? Vector3.Cross(Vector3.up, dirToPlayer) : Vector3.Cross(dirToPlayer, Vector3.up);

        Vector3 dodgePos = transform.position + dodgeDir * dodgeDistance;

        // Giữ y trên mặt đất NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(dodgePos, out hit, dodgeDistance, NavMesh.AllAreas))
        {
            return hit.position;
        }

        // Nếu không tìm được vị trí hợp lệ thì trả về vị trí hiện tại
        return transform.position;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcFire()
    {
        if (firePrefab == null || firePoint == null)
        {
            Debug.LogWarning("firePrefab hoặc firePoint chưa được gán!");
            return;
        }

        NetworkObject bullet = Runner.Spawn(firePrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        if (rb != null)
        {
            GameObject targetPlayer = FindClosestPlayer();
            if (targetPlayer == null)
            {
                Debug.LogWarning("Không tìm thấy player để bắn!");
                return;
            }
            // Lấy vị trí mục tiêu là vị trí player + offset chiều cao
            Vector3 targetPos = targetPlayer.transform.position + Vector3.up * 1.5f; // 1.5f là chiều cao bạn muốn, chỉnh cho phù hợp

            Vector3 direction = (targetPos - firePoint.position).normalized;

            rb.useGravity = false;
            rb.AddForce(direction * 20f, ForceMode.Impulse);

            Quaternion lookRot = Quaternion.LookRotation(direction);
            Quaternion correction = Quaternion.Euler(90, 0, 0);
            rb.transform.rotation = lookRot * correction;
        }

        StartCoroutine(DestroyAfterSeconds(bullet, 2f));
    }

    private IEnumerator DestroyAfterSeconds(NetworkObject obj, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (obj != null && obj.IsValid)
            Runner.Despawn(obj);
    }
}
