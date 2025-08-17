using Fusion;
using UnityEngine;

public class PlayerSceneSpawn : SimulationBehaviour, IPlayerJoined
{
    [Header("Player Prefab")]
    public NetworkPrefabRef playerPrefab;

    public void PlayerJoined(PlayerRef player)
    {
        // Tạo vị trí spawn mặc định
        Vector3 spawnPos = Vector3.zero;

        // Nếu có lưu vị trí spawn thì lấy
        if (PlayerPrefs.HasKey("SpawnX"))
        {
            float x = PlayerPrefs.GetFloat("SpawnX");
            float y = PlayerPrefs.GetFloat("SpawnY");
            float z = PlayerPrefs.GetFloat("SpawnZ");

            spawnPos = new Vector3(x, y, z);
        }

        // Spawn player mới
        NetworkObject playerObj = Runner.Spawn(playerPrefab, spawnPos, Quaternion.identity, player);
        Debug.Log($"Spawned player {player} at {spawnPos}");
    }
}
