using System.Linq;
using Fusion;
using UnityEngine;
using UnityEngine.AI;

public class SpawEnemyNetwork : NetworkBehaviour

{
    [Header("Spawn Enemy")]
    public Transform[] transformsEnemy;
    
    public GameObject[] enemyPrefab;
    public override void Spawned()
    {
       
        // Chỉ thực hiện khi có quyền sở hữu trạng thái
        if (Object.HasStateAuthority)
        {
           
            for (int i = 0; i < transformsEnemy.Count(); i++)
            {
                var randomIndex = Random.Range(0, enemyPrefab.Length);
                Runner.Spawn(enemyPrefab[randomIndex], transformsEnemy[i].position, transformsEnemy[i].rotation);
                   
               
               
            }

        }
    }
}
