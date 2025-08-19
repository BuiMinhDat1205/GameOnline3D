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

    [Header("Enemy Counter UI")]
    public TextMeshProUGUI enemyCounterText; // UI hiển thị số quái còn lại
    public TextMeshProUGUI scoreText;        // 🆕 UI hiển thị điểm

    [Networked]
    public int RemainingEnemies { get; set; }

    [Networked] // 🆕 đồng bộ điểm
    public int Score { get; set; }

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            int count = 0;

            for (int i = 0; i < transformsEnemy.Count(); i++)
            {
                var randomIndex = Random.Range(0, enemyPrefab.Length);
                var enemyObj = Runner.Spawn(enemyPrefab[randomIndex], transformsEnemy[i].position, transformsEnemy[i].rotation);

                var hp = enemyObj.GetComponent<NetworkHealthEnemy>();
                if (hp != null) hp.spawner = this;

                count++;
            }

            RemainingEnemies = count;
            Score = 0; // 🆕 reset điểm khi bắt đầu
        }

        UpdateEnemyUI();
        UpdateScoreUI(); // 🆕 cập nhật điểm ban đầu
    }

    // 🆕 gọi khi enemy chết
    public void EnemyDied()
    {
        if (Object.HasStateAuthority)
        {
            RemainingEnemies--;
            if (RemainingEnemies < 0) RemainingEnemies = 0;

            // 🆕 cộng điểm
            int randomScore = UnityEngine.Random.Range(10, 51); // 51 để bao gồm 50
            Score += randomScore;
        }

        UpdateEnemyUI();
        UpdateScoreUI();
    }

    private void UpdateEnemyUI()
    {
        if (enemyCounterText != null)
        {
            enemyCounterText.text = "Enemies: " + RemainingEnemies;
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + Score;
        }
    }
}
