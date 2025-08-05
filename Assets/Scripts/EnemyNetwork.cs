using System.Collections;
using Fusion;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNetwork : NetworkBehaviour
{
    [Header("Player Tagert")]
    public GameObject[] playerTagert;
    [Header("NavMesh Agent")]
    public NavMeshAgent navMeshAgent;
    [Header("Enemy Animator")]
    public Animator npcAnimator;
    [Header("Enemy Old Transform")]
    public Vector3 npcOldTransform;
    [Header("Transform Fire Attack")]
    public Transform fireAttackTransform;
    [Header("GameOject Fire Attack")]
    public GameObject fireAttackGameObject;
    [Header("Attack Event")]
    private float fireCooldown = 1f; // thời gian giữa các phát bắn (giây)
    private float fireTime = 0f;
    public float distanceToPlayer = 20f;
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcSetAnimatorRunNPC(float i)
    {
        npcAnimator.SetFloat("NPCRun", i);
        Debug.Log("Set Animator Run NPC");
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcSetAnimatorAttackNPC()
    {
        npcAnimator.SetTrigger("FireAttack");
       
        NetworkObject fire = Runner.Spawn(fireAttackGameObject, fireAttackTransform.position, Quaternion.identity);
        Rigidbody rigidbody = fire.GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            rigidbody.AddForce(fireAttackTransform.forward * 20f, ForceMode.Impulse);
        }
        StartCoroutine(DestroyFireBall(fire));
        Debug.Log("Set Animator Attack Enemy"); }
      
    private void Start()
    {
        npcOldTransform = transform.position;
    }
    public override void FixedUpdateNetwork()
    {
        playerTagert = GameObject.FindGameObjectsWithTag("Player");



        // Chỉ thực hiện logic nếu có quyền sở hữu trạng thái
        for (int i = 0; i < playerTagert.Length; i++)
        {
            if (playerTagert[i] != null)
            {
                if (Vector3.Distance(playerTagert[i].transform.position, transform.position) <= distanceToPlayer)
                {
                    navMeshAgent.speed = 6f;
                    npcAnimator.speed = 1.2f;
                    navMeshAgent.SetDestination(playerTagert[i].transform.position);
                    RpcSetAnimatorRunNPC(navMeshAgent.velocity.magnitude);
                    //  Debug.Log("NPC dang di chuyển đến người chơi: " + playerTagert[i].name);
                    if (Vector3.Distance(playerTagert[i].transform.position, transform.position) <= 10f)
                    {
                        navMeshAgent.speed = 0f;
                        RpcSetAnimatorRunNPC(0f);
                        //  Debug.Log("NPC đã đến gần người chơi: " + playerTagert[i].name);
                        fireTime += Runner.DeltaTime;
                        if (fireTime >= fireCooldown)
                        {
                            Vector3 direction = (playerTagert[i].transform.position - transform.position).normalized;
                            fireAttackTransform.rotation = Quaternion.LookRotation(direction);
                            transform.rotation = Quaternion.LookRotation(direction);
                            RpcSetAnimatorAttackNPC();
                            fireTime = 0f; // Reset thời gian sau khi bắn
                        }


                    }


                }
                else
                {

                    navMeshAgent.speed = 2f;
                    npcAnimator.speed = 1f;
                    navMeshAgent.SetDestination(npcOldTransform);
                    RpcSetAnimatorRunNPC(navMeshAgent.velocity.magnitude);
                    //Debug.Log("NPC dang di chuyen ve vi tri cu: " + npcOldTransform);
                }

            }
        }
    }

    public IEnumerator DestroyFireBall(NetworkObject b)
    {
        yield return new WaitForSeconds(2f);
        Runner.Despawn(b);
    }
    public void SetDistancePlayer(float i)
    {
        distanceToPlayer = i;
    }
}
