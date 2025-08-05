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

    private void Start()
    {
        startPosition = transform.position;
    }

    public override void FixedUpdateNetwork()
    {
        // Tìm tất cả player
        players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length == 0) return;

        // Tìm player gần nhất
        GameObject closest = null;
        float closestDist = Mathf.Infinity;

        foreach (var p in players)
        {
            float dist = Vector3.Distance(p.transform.position, transform.position);
            if (dist < closestDist)
            {
                closest = p;
                closestDist = dist;
            }
        }

        if (closest != null && closestDist <= chaseDistance)
        {
            Vector3 targetPos = closest.transform.position;

            if (closestDist > attackDistance)
            {
                // Enemy đuổi theo
                agent.isStopped = false;
                agent.SetDestination(targetPos);
            }
            else
            {
                // Enemy đứng lại bắn
                agent.isStopped = true;

                fireTimer += Runner.DeltaTime;
                if (fireTimer >= fireCooldown)
                {
                    // Quay mặt về phía player
                    Vector3 dir = (targetPos - transform.position).normalized;
                    transform.rotation = Quaternion.LookRotation(dir);
                    firePoint.rotation = Quaternion.LookRotation(dir);

                    RpcFire();
                    fireTimer = 0f;
                }
            }
        }
        else
        {
            // Không thấy player, quay về vị trí ban đầu
            agent.isStopped = false;
            agent.SetDestination(startPosition);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcFire()
    {
        NetworkObject bullet = Runner.Spawn(firePrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Tính hướng từ firePoint đến player
            GameObject targetPlayer = FindClosestPlayer();
            if (targetPlayer != null)
            {
                Vector3 direction = (targetPlayer.transform.position - firePoint.position).normalized;
                rb.useGravity = false; // nếu muốn đạn bay thẳng
                rb.AddForce(direction * 20f, ForceMode.Impulse);
            }
        }

        StartCoroutine(DestroyAfterSeconds(bullet, 2f));
    }

    private GameObject FindClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject closest = null;
        float closestDist = Mathf.Infinity;

        foreach (var p in players)
        {
            float dist = Vector3.Distance(p.transform.position, transform.position);
            if (dist < closestDist)
            {
                closest = p;
                closestDist = dist;
            }
        }

        return closest;
    }

    private IEnumerator DestroyAfterSeconds(NetworkObject obj, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Runner.Despawn(obj);
    }
}
