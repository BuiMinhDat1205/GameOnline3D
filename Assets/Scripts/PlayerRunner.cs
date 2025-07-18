using Fusion;
using UnityEngine;

public class PlayerRunner : SimulationBehaviour, IPlayerJoined
{
    public GameObject PlayerPrefabs;

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            // Spawn player có gán quyền điều khiển đúng (inputAuthority)
            Runner.Spawn(PlayerPrefabs, new Vector3(6, 1, 6), Quaternion.identity, player);
        }
    }
}
