using System.Linq;
using Fusion;
using UnityEngine;
using UnityEngine.AI;
using TMPro; // 🆕 thêm để dùng TextMeshPro

public class SpawEnemyNetwork : NetworkBehaviour
{
    [Header("Spawn Enemy")]
    public Transform[] transformsEnemy;
    public GameObject[] enemyPrefab;

    [Header("Enemy Counter UI")] // 🆕
    public TextMeshProUGUI enemyCounterText; // kéo UI Text vào Inspector

    [Networked] // 🆕
    public int RemainingEnemies { get; set; }

    public override void Spawned()
    {
        // Chỉ thực hiện khi có quyền sở hữu trạng thái
        if (Object.HasStateAuthority)
        {
            int count = 0; // 🆕 đếm số enemy

            for (int i = 0; i < transformsEnemy.Count(); i++)
            {
                var randomIndex = Random.Range(0, enemyPrefab.Length);
                var enemyObj = Runner.Spawn(enemyPrefab[randomIndex], transformsEnemy[i].position, transformsEnemy[i].rotation);

                // 🆕 gán spawner để enemy báo ngược lại khi chết
                var hp = enemyObj.GetComponent<NetworkHealthEnemy>();
                if (hp != null) hp.spawner = this;

                count++;
            }

            RemainingEnemies = count; // 🆕 gán số enemy ban đầu
        }

        UpdateEnemyUI(); // 🆕 cập nhật UI ban đầu
    }

    // 🆕 Hàm gọi khi enemy chết
    public void EnemyDied()
    {
        if (Object.HasStateAuthority)
        {
            RemainingEnemies--;
            if (RemainingEnemies < 0) RemainingEnemies = 0;
        }
        UpdateEnemyUI();
    }

    // 🆕 cập nhật text UI
    private void UpdateEnemyUI()
    {
        if (enemyCounterText != null)
        {
            enemyCounterText.text = "Enemies: " + RemainingEnemies;
        }
    }
}
